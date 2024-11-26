namespace TrailerDownloader.Infrastructure.Services;

using System.IO;
using Microsoft.Extensions.Logging;
using TrailerDownloader.Application.Interfaces;
using TrailerDownloader.Domain.Common;
using TrailerDownloader.Domain.Models;

public class MovieService(
    ITmdbService tmdbService,
    IYoutubeService youtubeService,
    ILogger<MovieService> logger) : IMovieService
{
    private readonly List<Movie> _movies = [];  // Temporary in-memory storage

    public Task<Result<IEnumerable<Movie>>> GetAllMoviesAsync()
    {
        return Task.FromResult(Result<IEnumerable<Movie>>.Success(_movies.AsEnumerable()));
    }

    public Task<Result<Movie>> GetMovieByIdAsync(Guid id)
    {
        var movie = _movies.FirstOrDefault(m => m.Id == id);
        return Task.FromResult(movie is not null
            ? Result<Movie>.Success(movie)
            : Result<Movie>.Failure($"Movie with ID {id} not found"));
    }

    public async Task<Result<Movie>> AddMovieAsync(Movie movie)
    {
        try
        {
            var tmdbResult = await tmdbService.GetMovieDetailsAsync(movie.Title, movie.Year);
            if (!tmdbResult.IsSuccess)
                return Result<Movie>.Failure(tmdbResult.Error!);

            var enrichedMovie = tmdbResult.Value! with
            {
                Id = Guid.NewGuid(),
                FilePath = movie.FilePath,
                TmdbId = movie.TmdbId
            };

            _movies.Add(enrichedMovie);
            return Result<Movie>.Success(enrichedMovie);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error adding movie {Title}", movie.Title);
            return Result<Movie>.Failure($"Error adding movie: {ex.Message}");
        }
    }

    public async Task<Result<bool>> DownloadTrailerAsync(Guid movieId)
    {
        try
        {
            var movie = _movies.FirstOrDefault(m => m.Id == movieId);
            if (movie is null)
                return Result<bool>.Failure($"Movie with ID {movieId} not found");

            var trailerUrl = await tmdbService.GetMovieTrailerUrlAsync(movie.TmdbId);
            if (!trailerUrl.IsSuccess)
                return Result<bool>.Failure(trailerUrl.Error!);

            var outputPath = Path.Combine(
                Path.GetDirectoryName(movie.FilePath!)!,
                $"{movie.Title} ({movie.Year}) - Trailer.mp4");

            var downloadResult = await youtubeService.DownloadVideoAsync(trailerUrl.Value!, outputPath);
            if (!downloadResult.IsSuccess)
                return Result<bool>.Failure(downloadResult.Error!);

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error downloading trailer for movie {MovieId}", movieId);
            return Result<bool>.Failure($"Error downloading trailer: {ex.Message}");
        }
    }

    public Task<Result<bool>> ScanMediaDirectoriesAsync()
    {
        // TODO: Implement media directory scanning
        return Task.FromResult(Result<bool>.Success(true));
    }
}

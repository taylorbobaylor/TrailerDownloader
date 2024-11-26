namespace TrailerDownloader.Infrastructure.Services;

using Microsoft.Extensions.Options;
using TMDbLib.Client;
using TrailerDownloader.Application.Configuration;
using TrailerDownloader.Application.Interfaces;
using TrailerDownloader.Domain.Common;
using TrailerDownloader.Domain.Models;

public class TmdbService(IOptions<TmdbOptions> options) : ITmdbService
{
    private readonly TMDbClient _client = new(options.Value.ApiKey);

    public async Task<Result<Movie>> GetMovieDetailsAsync(string title, int year)
    {
        try
        {
            var searchResult = await _client.SearchMovieAsync(title);
            var movie = searchResult.Results.FirstOrDefault(m => m.ReleaseDate?.Year == year);

            if (movie is null)
                return Result<Movie>.Failure($"Movie not found: {title} ({year})");

            return Result<Movie>.Success(new Movie
            {
                Id = Guid.NewGuid(),
                Title = movie.Title,
                Year = movie.ReleaseDate?.Year ?? year,
                PosterPath = movie.PosterPath
            });
        }
        catch (Exception ex)
        {
            return Result<Movie>.Failure($"Error fetching movie details: {ex.Message}");
        }
    }

    public async Task<Result<string>> GetMovieTrailerUrlAsync(int tmdbId)
    {
        try
        {
            var videos = await _client.GetMovieVideosAsync(tmdbId);
            var trailer = videos.Results.FirstOrDefault(v =>
                v.Type == "Trailer" && v.Site == "YouTube");

            return trailer is not null
                ? Result<string>.Success($"https://www.youtube.com/watch?v={trailer.Key}")
                : Result<string>.Failure("No trailer found");
        }
        catch (Exception ex)
        {
            return Result<string>.Failure($"Error fetching trailer: {ex.Message}");
        }
    }
}

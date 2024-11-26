namespace TrailerDownloader.Application.Interfaces;

using TrailerDownloader.Domain.Common;
using TrailerDownloader.Domain.Models;

public interface ITmdbService
{
    Task<Result<Movie>> GetMovieDetailsAsync(string title, int year);
    Task<Result<string>> GetMovieTrailerUrlAsync(int tmdbId);
}

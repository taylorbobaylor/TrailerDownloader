namespace TrailerDownloader.Application.Interfaces;

using TrailerDownloader.Domain.Common;
using TrailerDownloader.Domain.Models;

public interface IMovieService
{
    Task<Result<IEnumerable<Movie>>> GetAllMoviesAsync();
    Task<Result<Movie>> GetMovieByIdAsync(Guid id);
    Task<Result<Movie>> AddMovieAsync(Movie movie);
    Task<Result<bool>> DownloadTrailerAsync(Guid movieId);
    Task<Result<bool>> ScanMediaDirectoriesAsync();
}

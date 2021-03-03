using System.Threading.Tasks;
using TrailerDownloader.Models;

namespace TrailerDownloader.Repositories.Interfaces
{
    public interface IMovieRepository
    {
        Task<Movie> GetMovieInfoAsync(Movie movie);
        Task<Movie> GetMovieFromDb(string path);
        Task SaveMoviesAsync(Movie[] movies);
        Task UpdateMovieAsync(Movie movie);
        Task<Movie> DownloadTrailerAsync(Movie movie);
        Task DeleteAllTrailersAsync(Movie movie);
    }
}

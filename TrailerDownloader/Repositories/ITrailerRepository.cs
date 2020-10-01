using System.Collections.Generic;
using System.Threading.Tasks;
using TrailerDownloader.Models;

namespace TrailerDownloader.Repositories
{
    public interface ITrailerRepository
    {
        bool GetAllMoviesInfo();
        bool DownloadAllTrailers(IEnumerable<Movie> movieList);
        Task<bool> DeleteAllTrailers(IEnumerable<Movie> movieList);
    }
}

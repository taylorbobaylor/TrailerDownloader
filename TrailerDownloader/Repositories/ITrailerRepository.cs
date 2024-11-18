using System.Collections.Generic;
using System.Threading.Tasks;
using TrailerDownloader.Models;

namespace TrailerDownloader.Repositories
{
    public interface ITrailerRepository
    {
        Task GetAllMoviesInfo();
        Task DownloadAllTrailers(IEnumerable<Movie> movieList);
        bool DeleteAllTrailers(IEnumerable<Movie> movieList);
    }
}

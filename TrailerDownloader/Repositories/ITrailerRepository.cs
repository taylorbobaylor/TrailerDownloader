using System.Collections.Generic;
using TrailerDownloader.Models;

namespace TrailerDownloader.Repositories
{
    public interface ITrailerRepository
    {
        void GetAllMoviesInfo();
        void DownloadAllTrailers(IEnumerable<Movie> movieList);
        bool DeleteAllTrailers(IEnumerable<Movie> movieList);
    }
}

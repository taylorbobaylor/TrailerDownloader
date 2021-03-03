using System.Collections.Generic;
using TrailerDownloader.Models;

namespace TrailerDownloader.Services.Interfaces
{
    public interface IMovieService
    {
        List<Movie> GetAllMovieInfo();
        void DownloadTrailers(Movie[] movies);
        void DeleteAllTrailers(Movie[] movies);
    }
}

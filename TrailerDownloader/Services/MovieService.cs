using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TrailerDownloader.Models;
using TrailerDownloader.Repositories.Interfaces;
using TrailerDownloader.Services.Interfaces;

namespace TrailerDownloader.Services
{
    public class MovieService : IMovieService
    {
        private readonly IConfiguration _config;
        private readonly IConfigService _configService;
        private readonly IMovieRepository _movieRepo;
        private static List<string> _allMovieDirectories;

        public MovieService(IConfiguration config, IConfigService configService, IMovieRepository movieRepo)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _configService = configService ?? throw new ArgumentNullException(nameof(configService));
            _movieRepo = movieRepo ?? throw new ArgumentNullException(nameof(movieRepo));

            _allMovieDirectories = new List<string>();
        }

        public List<Movie> GetAllMovieInfo()
        {
            GetAllMovieDirectories(_configService.GetConfig().BaseMediaPath);
            return GetMovieInfoFromDbOrAPI().Result;
        }

        private async Task<List<Movie>> GetMovieInfoFromDbOrAPI()
        {
            List<Task<Movie>> getMovieTaskList = new List<Task<Movie>>();
            List<Movie> movieList = new List<Movie>();

            foreach (string path in _allMovieDirectories)
            {
                Movie movie = await _movieRepo.GetMovieFromDb(path);
                if (movie == null)
                {
                    movie = MapMovieFromPath(path);
                    getMovieTaskList.Add(_movieRepo.GetMovieInfoAsync(movie));
                }

                movieList.Add(movie);
            }

            if (getMovieTaskList.Count > 0)
            {
                Movie[] movies = await Task.WhenAll(getMovieTaskList);
                await _movieRepo.SaveMoviesAsync(movies);
            }

            return movieList;
        }

        public void DownloadTrailers(Movie[] movies)
        {
            foreach (Movie movie in movies.OrderBy(movie => movie.Title))
            {
                if (movie.TrailerExists == false && string.IsNullOrEmpty(movie.TrailerURL) == false)
                {
                    if (_movieRepo.DownloadTrailerAsync(movie).Result.TrailerExists)
                    {
                        _movieRepo.UpdateMovieAsync(movie);
                    }
                }
            }
        }

        public void DeleteAllTrailers(Movie[] movies)
        {
            foreach (Movie movie in movies)
            {
                _movieRepo.DeleteAllTrailersAsync(movie);
            }
        }

        private void GetAllMovieDirectories(string basePath)
        {
            try
            {
                foreach (string path in Directory.GetDirectories(basePath))
                {
                    if (Directory.GetFiles(path).Length == 0)
                    {
                        GetAllMovieDirectories(path);
                    }
                    else
                    {
                        _allMovieDirectories.Add(path);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error in MovieInfoService.GetAllMovieDirectories");
            }
        }

        private Movie MapMovieFromPath(string path)
        {
            List<string> excludedTypes = _config.GetSection("ExcludedFileExtensions").Get<List<string>>();

            bool trailerExists = Directory.GetFiles(path).Where(name => name.Contains("-trailer")).Count() > 0;
            string filePath = Directory.GetFiles(path).FirstOrDefault(file => !excludedTypes.Any(x => file.EndsWith(x)) && !file.Contains("-trailer"));
            string title = Regex.Replace(Path.GetFileNameWithoutExtension(filePath), @"\(([^\)]+)\)", string.Empty).Trim().Replace("-trailer", string.Empty);
            string year = Regex.Replace(Path.GetFileNameWithoutExtension(filePath), @"^[^\(]+", string.Empty).Trim().Replace("(", string.Empty).Replace(")", string.Empty);

            return new Movie
            {
                TrailerExists = trailerExists,
                FilePath = path,
                Title = title,
                Year = year
            };
        }
    }
}

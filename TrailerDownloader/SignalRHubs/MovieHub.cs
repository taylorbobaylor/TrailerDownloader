using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TrailerDownloader.Models;
using TrailerDownloader.Repositories;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace TrailerDownloader.SignalRHubs
{
    public class MovieHub : Hub, ITrailerRepository
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<MovieHub> _logger;

        // Configs from JSON
        private static string _mediaDirectory;
        private static string _apiKey;
        private static readonly string _configPath = Path.Combine(Directory.GetCurrentDirectory(), "config.json");

        public MovieHub(IHttpClientFactory httpClientFactory, ILogger<MovieHub> logger)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            if (File.Exists(_configPath))
            {
                string jsonConfig = File.ReadAllText(_configPath);
                _mediaDirectory = JsonConvert.DeserializeObject<Config>(jsonConfig).MediaDirectory;
                _apiKey = JsonConvert.DeserializeObject<Config>(jsonConfig).TMDBKey;
            }
        }

        public async Task<bool> GetAllMoviesInfo()
        {
            IEnumerable<Movie> movieList;
            List<Task<Movie>> taskList = new List<Task<Movie>>();

            foreach (string movieDirectory in Directory.GetDirectories(_mediaDirectory))
            {
                bool trailerExists = Directory.GetFiles(movieDirectory).Where(name => name.Contains("-trailer")).Count() > 0;
                string filePath = Directory.GetFiles(movieDirectory).Where(ext => !ext.EndsWith("srt") || !ext.EndsWith("sub") || !ext.EndsWith("sbv") || !ext.Contains("-trailer")).FirstOrDefault();
                string title = Regex.Replace(Path.GetFileNameWithoutExtension(filePath), @"\(([^\)]+)\)", string.Empty).Trim().Replace("-trailer", string.Empty);
                string year = Regex.Replace(Path.GetFileNameWithoutExtension(filePath), @"^[^\(]+", string.Empty).Trim().Replace("(", string.Empty).Replace(")", string.Empty);

                Movie movieInfo = new Movie
                {
                    TrailerExists = trailerExists,
                    FilePath = Path.GetDirectoryName(filePath),
                    Title = title,
                    Year = year
                };

                taskList.Add(GetMovieInfoAsync(movieInfo));
            }

            movieList = await Task.WhenAll(taskList);

            await Clients.All.SendAsync("completedAllMoviesInfo", movieList.Count());
            return true;
        }

        public async Task<bool> DownloadAllTrailers(IEnumerable<Movie> movieList)
        {
            foreach (Movie movie in movieList)
            {
                if (movie.TrailerExists == false && string.IsNullOrEmpty(movie.TrailerURL) == false)
                {
                    if (DownloadTrailerAsync(movie).Result)
                    {
                        movie.TrailerExists = true;
                    }

                    await Clients.All.SendAsync("downloadAllTrailers", movieList);
                }
            }

            await Clients.All.SendAsync("doneDownloadingAllTrailersListener", true);

            return true;
        }

        public bool DeleteAllTrailers(IEnumerable<Movie> movieList)
        {
            ParallelLoopResult result = Parallel.ForEach(movieList, async movie =>
            {
                if (movie.TrailerExists)
                {
                    string filePath = Directory.GetFiles(movie.FilePath).Where(name => name.Contains("-trailer")).FirstOrDefault();
                    File.Delete(filePath);
                    movie.TrailerExists = false;
                    await Clients.All.SendAsync("deleteAllTrailers", movie);
                }
            });

            return result.IsCompleted;
        }

        private async Task<bool> DownloadTrailerAsync(Movie movie)
        {
            try
            {
                YoutubeClient youtube = new YoutubeClient();
                StreamManifest streamManifest = await youtube.Videos.Streams.GetManifestAsync(movie.TrailerURL);

                // Get highest quality muxed stream
                IVideoStreamInfo streamInfo = streamManifest.GetMuxed().WithHighestVideoQuality();

                if (streamInfo != null)
                {
                    // Download the stream to file
                    await youtube.Videos.Streams.DownloadAsync(streamInfo, Path.Combine(movie.FilePath, $"{movie.Title}-trailer.{streamInfo.Container}"));

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error downloading trailer for {movie.Title}\n{ex.Message}");
                return false;
                //throw new Exception($"Error downloading trailer for {movie.Title}\n{ex.Message}");
            }
        }

        private async Task<Movie> GetMovieInfoAsync(Movie movie)
        {
            HttpClient httpClient = _httpClientFactory.CreateClient();

            string uri = $"https://api.themoviedb.org/3/search/movie?language=en-US&query={movie.Title}&year={movie.Year}&api_key={_apiKey}";
            HttpResponseMessage response = await httpClient.GetAsync(new Uri(uri));

            if (response.IsSuccessStatusCode)
            {
                JToken results = JsonConvert.DeserializeObject<JObject>(await response.Content.ReadAsStringAsync()).GetValue("results");
                JToken singleResult = results.Where(j => j.Value<string>("title") == movie.Title).FirstOrDefault();

                if (singleResult != null)
                {
                    movie.PosterPath = $"https://image.tmdb.org/t/p/w500/{singleResult.Value<string>("poster_path")}";
                    movie.Id = singleResult.Value<int>("id");
                }
                else if (results != null)
                {
                    movie.PosterPath = $"https://image.tmdb.org/t/p/w500/{results.First?.Value<string>("poster_path")}";
                    movie.Id = results.First?.Value<int>("id");
                }

                movie.TrailerURL = await GetTrailerURL(movie.Id);
                await Clients.All.SendAsync("getAllMoviesInfo", movie);

                return movie;
            }

            return new Movie();
        }

        private async Task<string> GetTrailerURL(int? id)
        {
            if (id != null)
            {
                HttpClient httpClient = _httpClientFactory.CreateClient();
                string uri = $"https://api.themoviedb.org/3/movie/{id}/videos?api_key={_apiKey}&language=en-US";

                HttpResponseMessage response = await httpClient.GetAsync(new Uri(uri));
                if (response.IsSuccessStatusCode)
                {
                    JToken results = JsonConvert.DeserializeObject<JObject>(await response.Content.ReadAsStringAsync()).GetValue("results");
                    if (results.Count() != 0)
                    {
                        return results.First.Value<string>("key");
                    }
                }
            }

            return string.Empty;
        }
    }
}

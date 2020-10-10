using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
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
        private static readonly ConcurrentDictionary<string, Movie> _movieDictionary = new ConcurrentDictionary<string, Movie>();

        private static readonly string _apiKey = "e438e2812f17faa299396505f2b375bb";
        private static readonly string _configPath = Path.Combine(Directory.GetCurrentDirectory(), "config.json");
        private static string _mediaDirectory;

        public MovieHub(IHttpClientFactory httpClientFactory, ILogger<MovieHub> logger)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            if (File.Exists(_configPath))
            {
                string jsonConfig = File.ReadAllText(_configPath);
                _mediaDirectory = JsonConvert.DeserializeObject<Config>(jsonConfig).MediaDirectory;
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

                Movie movie = new Movie
                {
                    TrailerExists = trailerExists,
                    FilePath = Path.GetDirectoryName(filePath),
                    Title = title,
                    Year = year
                };

                if (_movieDictionary.TryGetValue(movie.FilePath, out Movie dictionaryMovie))
                {
                    await Clients.All.SendAsync("getAllMoviesInfo", dictionaryMovie);
                }
                else
                {
                    taskList.Add(GetMovieInfoAsync(movie));
                }
            }

            if (taskList.Count > 0)
            {
                movieList = await Task.WhenAll(taskList);
            }

            _movieDictionary.ToList().ForEach(mov =>
            {
                if (Directory.Exists(mov.Value.FilePath) == false)
                {
                    _ = _movieDictionary.TryRemove(mov.Value.FilePath, out Movie movie);
                }
            });

            await Clients.All.SendAsync("completedAllMoviesInfo", _movieDictionary.Count);
            return true;
        }

        public async Task<bool> DownloadAllTrailers(IEnumerable<Movie> movieList)
        {
            foreach (Movie movie in movieList.OrderBy(movie => movie.Title))
            {
                if (movie.TrailerExists == false && string.IsNullOrEmpty(movie.TrailerURL) == false)
                {
                    if (DownloadTrailerAsync(movie).Result)
                    {
                        movie.TrailerExists = true;
                        await Clients.All.SendAsync("downloadAllTrailers", movie);
                    }
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
                    _movieDictionary.FirstOrDefault(mov => mov.Value.FilePath == movie.FilePath).Value.TrailerExists = false;
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
                await Clients.All.SendAsync("downloadAllTrailers", movie);
                return false;
            }
        }

        private async Task<Movie> GetMovieInfoAsync(Movie movie)
        {
            try
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

                    movie = _movieDictionary.GetOrAdd(movie.FilePath, movie);
                    return movie;
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting movie info for {movie.Title}\n{ex.Message}");
                return null;
            }
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

using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        // Configs from JSON
        private static string _mediaDirectory;
        private static string _apiKey;
        private static readonly string configPath = Path.Combine(Directory.GetCurrentDirectory(), "config.json");

        public MovieHub(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));

            if (File.Exists(configPath))
            {
                string jsonConfig = File.ReadAllText(configPath);
                _mediaDirectory = JsonConvert.DeserializeObject<Config>(jsonConfig).MediaDirectory;
                _apiKey = JsonConvert.DeserializeObject<Config>(jsonConfig).TMDBKey;
            }
        }

        public async Task<bool> GetAllMoviesInfo()
        {
            List<Movie> movieList = new List<Movie>();

            ParallelLoopResult result = Parallel.ForEach(Directory.GetDirectories(_mediaDirectory), async movieDirectory =>
            {
                bool trailerExists = Directory.GetFiles(movieDirectory).Where(name => name.Contains("-Trailer")).Count() > 0;
                string filePath = Directory.GetFiles(movieDirectory).Where(ext => !ext.EndsWith("srt") || !ext.EndsWith("sub") || !ext.EndsWith("sbv") || !ext.Contains("-Trailer")).FirstOrDefault();
                string title = Regex.Replace(Path.GetFileNameWithoutExtension(filePath), @"\(([^\)]+)\)", string.Empty).Trim().Replace("-Trailer", string.Empty);
                string year = Regex.Replace(Path.GetFileNameWithoutExtension(filePath), @"^[^\(]+", string.Empty).Trim().Replace("(", string.Empty).Replace(")", string.Empty);

                Movie movieInfo = new Movie
                {
                    TrailerExists = trailerExists,
                    FilePath = Path.GetDirectoryName(filePath),
                    Title = title,
                    Year = year
                };

                movieList.Add(GetMovieInfoAsync(movieInfo).Result);
                await Clients.All.SendAsync("getAllMoviesInfo", movieList.OrderBy(m => m.Title));
            });

            await Clients.All.SendAsync("completedAllMoviesInfo", movieList.Count);
            return result.IsCompleted;
        }

        public bool DownloadAllTrailers(IEnumerable<Movie> movieList)
        {
            ParallelLoopResult result = Parallel.ForEach(movieList, async movie =>
            {
                if (movie.TrailerExists == false)
                {
                    if (DownloadTrailerAsync(movie).Result)
                    {
                        movie.TrailerExists = true;
                    }

                    await Clients.All.SendAsync("downloadAllTrailers", movieList);
                }
            });

            return result.IsCompleted;
        }

        public async Task<bool> DeleteAllTrailers(IEnumerable<Movie> movieList)
        {
            foreach (Movie movie in movieList)
            {
                if (movie.TrailerExists)
                {
                    string filePath = Directory.GetFiles(movie.FilePath).Where(name => name.Contains("-Trailer")).FirstOrDefault();
                    File.Delete(filePath);
                    movie.TrailerExists = false;
                    await Clients.All.SendAsync("deleteAllTrailers", movieList);
                }
            }

            return true;
        }

        private async Task<bool> DownloadTrailerAsync(Movie movie)
        {
            YoutubeClient youtube = new YoutubeClient();
            StreamManifest streamManifest = await youtube.Videos.Streams.GetManifestAsync(movie.TrailerURL);

            // Get highest quality muxed stream
            IVideoStreamInfo streamInfo = streamManifest.GetMuxed().WithHighestVideoQuality();

            if (streamInfo != null)
            {
                // Download the stream to file
                await youtube.Videos.Streams.DownloadAsync(streamInfo, Path.Combine(movie.FilePath, $"{movie.Title}-Trailer.{streamInfo.Container}"));

                return true;
            }

            return false;
        }

        private async Task<Movie> GetMovieInfoAsync(Movie movie)
        {
            HttpClient httpClient = _httpClientFactory.CreateClient();

            string uri = $"https://api.themoviedb.org/3/search/movie?language=en-US&query={movie.Title}&year={movie.Year}&api_key={_apiKey}";
            HttpResponseMessage response = await httpClient.GetAsync(new Uri(uri));

            if (response.IsSuccessStatusCode)
            {
                JToken results = JsonConvert.DeserializeObject<JObject>(await response.Content.ReadAsStringAsync()).GetValue("results");
                results = results.Where(j => j.Value<string>("title") == movie.Title).FirstOrDefault();

                if (results.Count() != 0)
                {
                    movie.PosterPath = $"https://image.tmdb.org/t/p/w500/" + results.Value<string>("poster_path");
                    movie.Id = results.Value<int>("id");
                    movie.TrailerURL = await GetTrailerURL(movie.Id);

                    return movie;
                }
            }

            return new Movie();
        }

        private async Task<string> GetTrailerURL(int id)
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

            return string.Empty;
        }
    }
}

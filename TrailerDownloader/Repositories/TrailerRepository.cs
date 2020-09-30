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

namespace TrailerDownloader.Repositories
{
    public class TrailerRepository : ITrailerRepository
    {
        private readonly IHttpClientFactory _httpClientFactory;

        // Configs from JSON
        private static string _mediaDirectory;
        private static string _apiKey;
        private static string configPath = Path.Combine(Directory.GetCurrentDirectory(), "config.json");

        public TrailerRepository(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));

            if (File.Exists(configPath))
            {
                string jsonConfig = File.ReadAllText(configPath);
                _mediaDirectory = JsonConvert.DeserializeObject<Config>(jsonConfig).MediaDirectory;
                _apiKey = JsonConvert.DeserializeObject<Config>(jsonConfig).TMDBKey;
            }
        }

        public async Task<IEnumerable<Movie>> GetAllMoviesInfoAsync()
        {
            List<Movie> movieList = new List<Movie>();

            foreach (string movieDirectory in Directory.GetDirectories(_mediaDirectory))
            {
                bool trailerExists = Directory.GetFiles(movieDirectory).Where(name => name.Contains("-Trailer")).Count() > 0;
                string filePath = Directory.GetFiles(movieDirectory).Where(ext => !ext.EndsWith("srt") || !ext.EndsWith("sub") || !ext.EndsWith("sbv") || !ext.Contains("-Trailer")).FirstOrDefault();
                string title = Regex.Replace(Path.GetFileNameWithoutExtension(filePath), @"\(([^\)]+)\)", string.Empty).Trim();
                string year = Regex.Replace(Path.GetFileNameWithoutExtension(filePath), @"^[^\(]+", string.Empty).Trim().Replace("(", string.Empty).Replace(")", string.Empty);

                Movie movieInfo = new Movie
                {
                    TrailerExists = trailerExists,
                    FilePath = Path.GetDirectoryName(filePath),
                    Title = title,
                    Year = year
                };

                movieList.Add(await GetMovieInfoAsync(movieInfo));
            }

            return movieList;
        }

        private async Task<Movie> GetMovieInfoAsync(Movie movie)
        {
            HttpClient httpClient = _httpClientFactory.CreateClient();

            string uri = $"https://api.themoviedb.org/3/search/movie?language=en-US&query={movie.Title}&year={movie.Year}&api_key={_apiKey}";
            HttpResponseMessage response = await httpClient.GetAsync(new Uri(uri));

            if (response.IsSuccessStatusCode)
            {
                JToken results = JsonConvert.DeserializeObject<JObject>(await response.Content.ReadAsStringAsync()).GetValue("results");

                if (results.Count() != 0)
                {
                    movie.PosterPath = $"https://image.tmdb.org/t/p/w500/" + results.First.Value<string>("poster_path");
                    movie.Id = results.First.Value<int>("id");
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

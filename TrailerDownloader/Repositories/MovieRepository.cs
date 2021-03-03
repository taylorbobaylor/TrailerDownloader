using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TrailerDownloader.Context;
using TrailerDownloader.Models;
using TrailerDownloader.Repositories.Interfaces;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace TrailerDownloader.Repositories
{
    public class MovieRepository : Hub, IMovieRepository
    {
        private readonly MovieDbContext _context;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly IHubContext<MovieRepository> _hubContext;

        public MovieRepository(MovieDbContext context, HttpClient httpClient, IConfiguration config, IHubContext<MovieRepository> hubContext)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
        }

        public async Task<Movie> GetMovieInfoAsync(Movie movie)
        {
            try
            {
                string uri = $"https://api.themoviedb.org/3/search/movie?language=en-US&query={movie.Title}&year={movie.Year}&api_key={_config.GetValue<string>("TMDBApiKey")}";
                HttpResponseMessage response = await _httpClient.GetAsync(new Uri(uri));

                if (response.IsSuccessStatusCode)
                {
                    JToken results = JsonConvert.DeserializeObject<JObject>(await response.Content.ReadAsStringAsync()).GetValue("results");
                    JToken singleResult = results.Where(j => j.Value<string>("title") == movie.Title).FirstOrDefault();

                    if (singleResult != null)
                    {
                        movie.PosterPath = $"https://image.tmdb.org/t/p/w500/{singleResult.Value<string>("poster_path")}";
                        movie.TMDBId = singleResult.Value<int>("id");
                    }
                    else if (results != null)
                    {
                        movie.PosterPath = $"https://image.tmdb.org/t/p/w500/{results.First?.Value<string>("poster_path")}";
                        movie.TMDBId = results.First.Value<int>("id");
                    }

                    movie.TrailerURL = await GetTrailerURL(movie.TMDBId);
                    await _hubContext.Clients.All.SendAsync("receiveMovieInfo", movie);

                    Log.Information($"Retrieved movie info from TMDB for: {movie.Title}");
                    return movie;
                }

                return null;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error getting movie info for {movie.Title}");
                throw;
            }
        }

        private async Task<string> GetTrailerURL(int? id)
        {
            if (id != null)
            {
                string uri = $"https://api.themoviedb.org/3/movie/{id}/videos?api_key={_config.GetValue<string>("TMDBApiKey")}&language=en-US";

                HttpResponseMessage response = await _httpClient.GetAsync(new Uri(uri));
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

        public async Task<Movie> GetMovieFromDb(string path)
        {
            Movie movie = _context.Movie.FirstOrDefault(x => x.FilePath == path);
            if (movie != null)
            {
                movie.TrailerExists = Directory.GetFiles(path).Where(name => name.Contains("-trailer")).Count() > 0;
                await _hubContext.Clients.All.SendAsync("receiveMovieInfo", movie);
            }
            return movie;
        }

        public async Task SaveMoviesAsync(Movie[] movies)
        {
            try
            {
                await _context.Movie.AddRangeAsync(movies);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error in MovieInfoRepository.SaveMoviesAsync");
                throw;
            }
        }

        public async Task UpdateMovieAsync(Movie movie)
        {
            try
            {
                _context.Movie.Update(movie);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error in MovieInfoRepository.UpdateMovieAsync");
                throw;
            }
        }

        public async Task<Movie> DownloadTrailerAsync(Movie movie)
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

                    movie.TrailerExists = true;
                    await _hubContext.Clients.All.SendAsync("downloadAllTrailers", movie);
                    Log.Information($"Successfully downloaded trailer for: {movie.Title}");
                }

                return movie;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error downloading trailer for {movie.Title}");
                await _hubContext.Clients.All.SendAsync("downloadAllTrailers", movie);
                return movie;
            }
        }

        public async Task DeleteAllTrailersAsync(Movie movie)
        {
            if (movie.TrailerExists)
            {
                string path = Directory.GetFiles(movie.FilePath).Where(name => name.Contains("-trailer")).FirstOrDefault();
                File.Delete(path);
                movie.TrailerExists = false;
                await _hubContext.Clients.All.SendAsync("receiveMovieInfo", movie);
                UpdateMovieAsync(movie).Wait();
            }
        }
    }
}

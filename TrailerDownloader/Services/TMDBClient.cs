using System;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;

namespace TrailerDownloader.Services
{
    /// <summary>
    /// TMDBClient is responsible for interacting with the TMDB API.
    /// </summary>
    public class TMDBClient
    {
        private readonly string _apiKey;
        private readonly string _apiBaseUrl = "https://api.themoviedb.org/3";

        /// <summary>
        /// Initializes a new instance of the TMDBClient class with the provided API key.
        /// </summary>
        /// <param name="apiKey">The API key for accessing TMDB API.</param>
        public TMDBClient(string apiKey)
        {
            _apiKey = apiKey;
        }

        /// <summary>
        /// Constructs the URL for the TMDB API endpoint with the API key included.
        /// </summary>
        /// <param name="pathSegment">The specific path segment of the API endpoint.</param>
        /// <returns>The complete URL with the API key query parameter.</returns>
        private string ConstructUrlWithApiKey(string pathSegment)
        {
            return _apiBaseUrl.AppendPathSegment(pathSegment).SetQueryParam("api_key", _apiKey);
        }

        /// <summary>
        /// Retrieves the configuration from the TMDB API.
        /// </summary>
        /// <returns>A dynamic object containing the configuration data.</returns>
        public async Task<dynamic> GetConfigurationAsync()
        {
            try
            {
                var configUrl = ConstructUrlWithApiKey("configuration");
                return await configUrl.GetJsonAsync<dynamic>();
            }
            catch (FlurlHttpException ex)
            {
                // Handle HTTP exceptions
                Console.WriteLine($"Failed to get configuration: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Searches for movies on TMDB that match the given query string.
        /// </summary>
        /// <param name="query">The search query string.</param>
        /// <returns>A dynamic object containing the search results.</returns>
        public async Task<dynamic> SearchMoviesAsync(string query)
        {
            try
            {
                var searchUrl = ConstructUrlWithApiKey("search/movie").SetQueryParam("query", query);
                return await searchUrl.GetJsonAsync<dynamic>();
            }
            catch (FlurlHttpException ex)
            {
                // Handle HTTP exceptions
                Console.WriteLine($"Failed to search movies: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Retrieves detailed information about a specific movie from TMDB.
        /// </summary>
        /// <param name="movieId">The unique identifier for the movie.</param>
        /// <returns>A dynamic object containing the movie details.</returns>
        public async Task<dynamic> GetMovieDetailsAsync(int movieId)
        {
            try
            {
                var detailsUrl = ConstructUrlWithApiKey($"movie/{movieId}");
                return await detailsUrl.GetJsonAsync<dynamic>();
            }
            catch (FlurlHttpException ex)
            {
                // Handle HTTP exceptions
                Console.WriteLine($"Failed to get movie details: {ex.Message}");
                throw;
            }
        }

        // Additional methods for downloading trailers and other interactions can be added here.
    }
}

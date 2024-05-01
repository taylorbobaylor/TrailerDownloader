using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

public class TmdbService
{
    private readonly IMemoryCache _cache;
    private readonly HttpClient _httpClient;
    private readonly ILogger<TmdbService> _logger;
    private readonly string _baseUrl = "https://api.themoviedb.org/3";
    private readonly string _cacheFolderPath = "Cache"; // Path to the cache folder
    private readonly string _apiKey;

    public TmdbService(IMemoryCache cache, HttpClient httpClient, ILogger<TmdbService> logger, IConfiguration configuration)
    {
        _cache = cache;
        _httpClient = httpClient;
        _logger = logger;
        _apiKey = configuration["TMDB_API_KEY"];
        if (!Directory.Exists(_cacheFolderPath))
        {
            Directory.CreateDirectory(_cacheFolderPath);
        }
    }

    public async Task<string> SearchMoviesAsync(string query)
    {
        var cacheKey = $"search-results-{query}";
        var cacheFilePath = Path.Combine(_cacheFolderPath, $"{cacheKey}.json");

        if (_cache.TryGetValue(cacheKey, out string cachedResults))
        {
            return cachedResults;
        }
        else
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/search/movie?api_key={_apiKey}&query={Uri.EscapeDataString(query)}");
                response.EnsureSuccessStatusCode();
                var searchResults = await response.Content.ReadAsStringAsync();
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(1));
                _cache.Set(cacheKey, searchResults, cacheEntryOptions);
                await File.WriteAllTextAsync(cacheFilePath, searchResults);
                return searchResults;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching movies with query {Query}", query);
                throw;
            }
        }
    }

    public async Task<string> GetMovieTrailerDownloadUrlAsync(int movieId)
    {
        var cacheKey = $"trailer-download-url-{movieId}";
        var cacheFilePath = Path.Combine(_cacheFolderPath, $"{cacheKey}.json");

        if (_cache.TryGetValue(cacheKey, out string cachedDownloadUrl))
        {
            return cachedDownloadUrl;
        }
        else
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/movie/{movieId}/videos?api_key={_apiKey}");
                response.EnsureSuccessStatusCode();
                var trailersJson = await response.Content.ReadAsStringAsync();
                using (JsonDocument doc = JsonDocument.Parse(trailersJson))
                {
                    var results = doc.RootElement.GetProperty("results");
                    foreach (var result in results.EnumerateArray())
                    {
                        if (result.GetProperty("type").GetString() == "Trailer")
                        {
                            var trailerDownloadUrl = result.GetProperty("key").GetString();
                            var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(1));
                            _cache.Set(cacheKey, trailerDownloadUrl, cacheEntryOptions);
                            await File.WriteAllTextAsync(cacheFilePath, trailerDownloadUrl);
                            return trailerDownloadUrl;
                        }
                    }
                }
                return null; // No trailer found
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting trailer download URL for movie ID {MovieId}", movieId);
                throw;
            }
        }
    }

    // ... (rest of the code remains unchanged)
}

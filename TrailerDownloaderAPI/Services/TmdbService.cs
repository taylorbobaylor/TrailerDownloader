using Microsoft.Extensions.Caching.Memory;
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

    public TmdbService(IMemoryCache cache, HttpClient httpClient, ILogger<TmdbService> logger)
    {
        _cache = cache;
        _httpClient = httpClient;
        _logger = logger;
        if (!Directory.Exists(_cacheFolderPath))
        {
            Directory.CreateDirectory(_cacheFolderPath);
        }
    }

    public async Task<string> GetMovieTrailersAsync(string movieId)
    {
        var cacheKey = $"movie-trailers-{movieId}";
        var cacheFilePath = Path.Combine(_cacheFolderPath, $"{cacheKey}.json");

        // Check if the cache file exists and is valid
        if (File.Exists(cacheFilePath) && IsCacheValid(cacheFilePath))
        {
            return await File.ReadAllTextAsync(cacheFilePath);
        }
        else
        {
            try
            {
                // Fetch from API and cache
                var trailers = await FetchMovieTrailersFromApiAsync(movieId);
                await File.WriteAllTextAsync(cacheFilePath, trailers);
                return trailers;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching movie trailers from API for movie ID {MovieId}", movieId);
                throw;
            }
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
                var response = await _httpClient.GetAsync($"{_baseUrl}/search/movie?query={Uri.EscapeDataString(query)}");
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
                var trailersJson = await GetMovieTrailersAsync(movieId.ToString());
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

    private bool IsCacheValid(string cacheFilePath)
    {
        var creationTime = File.GetCreationTime(cacheFilePath);
        return (DateTime.Now - creationTime) < TimeSpan.FromHours(1); // Cache is valid for 1 hour
    }

    private async Task<string> FetchMovieTrailersFromApiAsync(string movieId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/movie/{movieId}/videos");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching movie trailers from API for movie ID {MovieId}", movieId);
            throw;
        }
    }
}

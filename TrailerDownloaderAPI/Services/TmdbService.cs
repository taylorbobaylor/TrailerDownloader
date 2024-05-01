using Microsoft.Extensions.Caching.Memory;
using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

public class TmdbService
{
    private readonly IMemoryCache _cache;
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl = "https://api.themoviedb.org/3";
    private readonly string _apiKey; // This should be stored securely and injected through configuration
    private readonly string _cacheFolderPath = "Cache"; // Path to the cache folder

    public TmdbService(IMemoryCache cache, HttpClient httpClient, string apiKey)
    {
        _cache = cache;
        _httpClient = httpClient;
        _apiKey = apiKey;
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
            // Fetch from API and cache
            var trailers = await FetchMovieTrailersFromApiAsync(movieId);
            await File.WriteAllTextAsync(cacheFilePath, trailers);
            return trailers;
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
            var response = await _httpClient.GetAsync($"{_baseUrl}/search/movie?api_key={_apiKey}&query={Uri.EscapeDataString(query)}");
            response.EnsureSuccessStatusCode();
            var searchResults = await response.Content.ReadAsStringAsync();
            var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(1));
            _cache.Set(cacheKey, searchResults, cacheEntryOptions);
            await File.WriteAllTextAsync(cacheFilePath, searchResults);
            return searchResults;
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
    }

    private bool IsCacheValid(string cacheFilePath)
    {
        var creationTime = File.GetCreationTime(cacheFilePath);
        return (DateTime.Now - creationTime) < TimeSpan.FromHours(1); // Cache is valid for 1 hour
    }

    private async Task<string> FetchMovieTrailersFromApiAsync(string movieId)
    {
        var response = await _httpClient.GetAsync($"{_baseUrl}/movie/{movieId}/videos?api_key={_apiKey}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
}

using Microsoft.Extensions.Caching.Memory;
using System;
using System.Net.Http;
using System.Threading.Tasks;

public class TmdbService
{
    private readonly IMemoryCache _cache;
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl = "https://api.themoviedb.org/3";
    private readonly string _apiKey; // This should be stored securely and injected through configuration

    public TmdbService(IMemoryCache cache, HttpClient httpClient, string apiKey)
    {
        _cache = cache;
        _httpClient = httpClient;
        _apiKey = apiKey;
    }

    public async Task<string> GetMovieTrailersAsync(string movieId)
    {
        var cacheKey = $"movie-trailers-{movieId}";
        if (!_cache.TryGetValue(cacheKey, out string trailers))
        {
            trailers = await FetchMovieTrailersFromApiAsync(movieId);
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromHours(1)); // Cache for 1 hour, can be adjusted as needed
            _cache.Set(cacheKey, trailers, cacheEntryOptions);
        }
        return trailers;
    }

    private async Task<string> FetchMovieTrailersFromApiAsync(string movieId)
    {
        var response = await _httpClient.GetAsync($"{_baseUrl}/movie/{movieId}/videos?api_key={_apiKey}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
}

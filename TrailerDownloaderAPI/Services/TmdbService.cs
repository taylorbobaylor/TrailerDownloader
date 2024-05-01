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

        // Configure HttpClient with the TMDB API key
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
    }

    // ... (rest of the code remains unchanged)
}

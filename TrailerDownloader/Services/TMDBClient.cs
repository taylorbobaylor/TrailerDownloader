using System;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;

public class TMDBClient
{
    private readonly string _apiKey;
    private readonly string _apiBaseUrl = "https://api.themoviedb.org/3";

    public TMDBClient(string apiKey)
    {
        _apiKey = apiKey;
    }

    public async Task<dynamic> GetConfigurationAsync()
    {
        try
        {
            var configUrl = _apiBaseUrl.AppendPathSegment("configuration").SetQueryParam("api_key", _apiKey);
            return await configUrl.GetJsonAsync();
        }
        catch (FlurlHttpException ex)
        {
            // Handle HTTP exceptions
            Console.WriteLine($"Failed to get configuration: {ex.Message}");
            throw;
        }
    }

    public async Task<dynamic> SearchMoviesAsync(string query)
    {
        try
        {
            var searchUrl = _apiBaseUrl.AppendPathSegment("search/movie")
                                       .SetQueryParams(new { api_key = _apiKey, query = query });
            return await searchUrl.GetJsonAsync();
        }
        catch (FlurlHttpException ex)
        {
            // Handle HTTP exceptions
            Console.WriteLine($"Failed to search movies: {ex.Message}");
            throw;
        }
    }

    public async Task<dynamic> GetMovieDetailsAsync(int movieId)
    {
        try
        {
            var detailsUrl = _apiBaseUrl.AppendPathSegment($"movie/{movieId}").SetQueryParam("api_key", _apiKey);
            return await detailsUrl.GetJsonAsync();
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

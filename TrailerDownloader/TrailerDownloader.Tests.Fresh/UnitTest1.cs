using Xunit;
using TrailerDownloader.Services;
using System.Threading.Tasks;

namespace TrailerDownloader.Tests.Fresh
{
    public class UnitTest1
    {
        private readonly TMDBClient _tmdbClient;
        private readonly string _validApiKey = "your_api_key"; // Replace with a valid API key
        private readonly int _validMovieId = 550; // Replace with a valid movie ID for testing

        public UnitTest1()
        {
            _tmdbClient = new TMDBClient(_validApiKey);
        }

        // This test verifies that the TMDBClient can retrieve configuration data from the TMDB API.
        [Fact]
        public async Task GetConfigurationAsync_ReturnsConfigurationData()
        {
            // Arrange
            // The TMDBClient is already initialized with a valid API key

            // Act
            var configurationData = await _tmdbClient.GetConfigurationAsync();

            // Assert
            Assert.NotNull(configurationData);
            Assert.Contains("images", configurationData);
        }

        // This test verifies that the TMDBClient can search for movies based on a query string.
        [Fact]
        public async Task SearchMoviesAsync_ReturnsSearchResults()
        {
            // Arrange
            string query = "Fight Club"; // Replace with a query string relevant to your test case

            // Act
            var searchResults = await _tmdbClient.SearchMoviesAsync(query);

            // Assert
            Assert.NotNull(searchResults);
            Assert.Contains("results", searchResults);
            Assert.All(searchResults.results, item => Assert.Contains(query, item.title));
        }

        // This test verifies that the TMDBClient can retrieve details for a specific movie.
        [Fact]
        public async Task GetMovieDetailsAsync_ReturnsMovieDetails()
        {
            // Arrange
            // The _validMovieId is set to a known valid movie ID

            // Act
            var movieDetails = await _tmdbClient.GetMovieDetailsAsync(_validMovieId);

            // Assert
            Assert.NotNull(movieDetails);
            Assert.Equal(_validMovieId, (int)movieDetails.id);
        }
    }
}

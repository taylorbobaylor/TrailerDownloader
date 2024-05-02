using Xunit;
using WireMock.Server;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Matchers;
using System.Threading.Tasks;
using TrailerDownloader.Services;

namespace TrailerDownloader.Tests
{
    /// <summary>
    /// Contains unit tests for the TMDBClient class.
    /// </summary>
    public class TMDBClientTests
    {
        private readonly WireMockServer _server;
        private readonly TMDBClient _client;

        /// <summary>
        /// Constructor for TMDBClientTests.
        /// Sets up a mock server and initializes the TMDBClient with a test API key.
        /// </summary>
        public TMDBClientTests()
        {
            _server = WireMockServer.Start();
            _client = new TMDBClient("test_api_key");
            _client.BaseUrl = _server.Urls[0]; // Redirects API calls to the mock server
        }

        /// <summary>
        /// Tests that GetConfigurationAsync returns a valid configuration.
        /// </summary>
        [Fact]
        public async Task GetConfigurationAsync_ReturnsConfiguration()
        {
            // Arrange
            _server.Given(Request.Create().WithPath("/3/configuration").UsingGet())
                .RespondWith(Response.Create().WithSuccess().WithBody("{ 'images': { 'base_url': 'http://example.com/' } }"));

            // Act
            var result = await _client.GetConfigurationAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal("http://example.com/", result.images.base_url);
        }

        /// <summary>
        /// Tests that SearchMoviesAsync returns search results for a given query.
        /// </summary>
        [Fact]
        public async Task SearchMoviesAsync_ReturnsSearchResults()
        {
            // Arrange
            var query = "Inception";
            _server.Given(Request.Create().WithPath("/3/search/movie").WithParam("query", query).UsingGet())
                .RespondWith(Response.Create().WithSuccess().WithBody("{ 'results': [{ 'id': 1, 'title': 'Inception' }] }"));

            // Act
            var result = await _client.SearchMoviesAsync(query);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.results);
            Assert.Equal("Inception", result.results[0].title);
        }

        /// <summary>
        /// Tests that GetMovieDetailsAsync returns detailed information for a specific movie ID.
        /// </summary>
        [Fact]
        public async Task GetMovieDetailsAsync_ReturnsMovieDetails()
        {
            // Arrange
            var movieId = 1;
            _server.Given(Request.Create().WithPath($"/3/movie/{movieId}").UsingGet())
                .RespondWith(Response.Create().WithSuccess().WithBody("{ 'id': 1, 'title': 'Inception', 'release_date': '2010-07-16' }"));

            // Act
            var result = await _client.GetMovieDetailsAsync(movieId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.id);
            Assert.Equal("Inception", result.title);
            Assert.Equal("2010-07-16", result.release_date);
        }

        // Additional test methods can be added here.
    }
}

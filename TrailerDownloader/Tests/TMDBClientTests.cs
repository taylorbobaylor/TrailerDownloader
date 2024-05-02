using Xunit;
using WireMock.Server;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Matchers;
using System.Threading.Tasks;
using TrailerDownloader.Services;

namespace TrailerDownloader.Tests
{
    public class TMDBClientTests
    {
        private readonly WireMockServer _server;
        private readonly TMDBClient _client;

        public TMDBClientTests()
        {
            _server = WireMockServer.Start();
            _client = new TMDBClient("test_api_key");
            _client.BaseUrl = _server.Urls[0]; // Redirects API calls to the mock server
        }

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

        // Additional test methods for SearchMoviesAsync and GetMovieDetailsAsync will be added here.
    }
}

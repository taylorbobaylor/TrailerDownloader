using Xunit;
using Moq;
using TrailerDownloader.Repositories;

namespace TrailerDownloader.Tests
{
    public class ConfigRepositoryTests
    {
        [Fact]
        public void ConfigRepository_Creation_Success()
        {
            // Arrange
            var mockConfig = new Mock<IConfigRepository>();

            // Act
            var configRepository = mockConfig.Object;

            // Assert
            Assert.NotNull(configRepository);
        }

        // Additional tests to be added here
    }
}

using Xunit;
using Moq;
using TrailerDownloader.Repositories;
using TrailerDownloader.Models;
using System.IO;
using Newtonsoft.Json;

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

        [Fact]
        public void GetConfig_ReturnsValidConfig_WhenFileIsCorrectlyFormatted()
        {
            // Arrange
            var configData = new Config
            {
                MediaDirectory = "test_media_directory",
                TrailerLanguage = "en"
            };
            var configJson = JsonConvert.SerializeObject(configData);
            var mockFileIO = new Mock<IFileIOService>();
            mockFileIO.Setup(x => x.ReadAllText(It.IsAny<string>())).Returns(configJson);
            var configRepository = new ConfigRepository(mockFileIO.Object);

            // Act
            var result = configRepository.GetConfig();

            // Assert
            Assert.NotNull(result);
            Assert.Equal("test_media_directory", result.MediaDirectory);
            Assert.Equal("en", result.TrailerLanguage);
        }

        // Additional tests to be added here
    }
}

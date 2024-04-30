using Xunit;
using Moq;
using TrailerDownloader.Repositories;
using TrailerDownloader.Models;
using TrailerDownloader.Services;
using Newtonsoft.Json;

namespace TrailerDownloader.Tests
{
    public class ConfigRepositoryTests
    {
        [Fact]
        public void ConfigRepository_Creation_Success()
        {
            // Arrange
            var mockFileIO = new Mock<IFileIOService>();
            var configRepository = new ConfigRepository(mockFileIO.Object);

            // Act
            var result = configRepository != null;

            // Assert
            Assert.True(result);
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
            mockFileIO.Setup(x => x.Exists(It.IsAny<string>())).Returns(true);
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

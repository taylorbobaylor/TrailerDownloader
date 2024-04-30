using Xunit;
using Moq;
using TrailerDownloader.Repositories;
using TrailerDownloader.Models;
using TrailerDownloader.Services;
using Newtonsoft.Json;
using System;

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

        [Fact]
        public void SaveConfig_WritesConfigToFile_WhenConfigIsValid()
        {
            // Arrange
            var configData = new Config
            {
                MediaDirectory = "test_media_directory",
                TrailerLanguage = "en"
            };
            var mockFileIO = new Mock<IFileIOService>();
            var configRepository = new ConfigRepository(mockFileIO.Object);

            // Act
            var result = configRepository.SaveConfig(configData);

            // Assert
            Assert.True(result);
            mockFileIO.Verify(x => x.WriteAllText(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void SaveConfig_ReturnsFalse_WhenConfigIsNull()
        {
            // Arrange
            var mockFileIO = new Mock<IFileIOService>();
            var configRepository = new ConfigRepository(mockFileIO.Object);

            // Act
            var result = configRepository.SaveConfig(null);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void SaveConfig_HandlesIOException_WhenFileCannotBeWritten()
        {
            // Arrange
            var configData = new Config
            {
                MediaDirectory = "test_media_directory",
                TrailerLanguage = "en"
            };
            var mockFileIO = new Mock<IFileIOService>();
            mockFileIO.Setup(x => x.WriteAllText(It.IsAny<string>(), It.IsAny<string>()))
                      .Throws(new IOException());
            var configRepository = new ConfigRepository(mockFileIO.Object);

            // Act & Assert
            var exception = Record.Exception(() => configRepository.SaveConfig(configData));
            Assert.IsType<IOException>(exception);
        }
    }
}

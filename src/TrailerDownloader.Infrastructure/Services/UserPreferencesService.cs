namespace TrailerDownloader.Infrastructure.Services;

using Microsoft.Extensions.Logging;
using TrailerDownloader.Application.Interfaces;
using TrailerDownloader.Domain.Common;
using TrailerDownloader.Domain.Models;

public class UserPreferencesService(ILogger<UserPreferencesService> logger) : IUserPreferencesService
{
    private UserPreferences _preferences = new()
    {
        Id = Guid.NewGuid(),
        MediaDirectories = ["/movies"],
        VideoFileExtensions = [".mp4", ".mkv", ".avi"],
        DownloadPath = "/movies/trailers",
        AutoDownload = false,
        TrailerLanguage = "en-US"
    };

    public Task<Result<UserPreferences>> GetUserPreferencesAsync()
    {
        return Task.FromResult(Result<UserPreferences>.Success(_preferences));
    }

    public Task<Result<UserPreferences>> UpdateUserPreferencesAsync(UserPreferences preferences)
    {
        try
        {
            _preferences = preferences;
            return Task.FromResult(Result<UserPreferences>.Success(_preferences));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating user preferences");
            return Task.FromResult(Result<UserPreferences>.Failure($"Error updating preferences: {ex.Message}"));
        }
    }
}

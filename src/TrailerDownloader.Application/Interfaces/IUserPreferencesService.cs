namespace TrailerDownloader.Application.Interfaces;

using TrailerDownloader.Domain.Common;
using TrailerDownloader.Domain.Models;

public interface IUserPreferencesService
{
    Task<Result<UserPreferences>> GetUserPreferencesAsync();
    Task<Result<UserPreferences>> UpdateUserPreferencesAsync(UserPreferences preferences);
}

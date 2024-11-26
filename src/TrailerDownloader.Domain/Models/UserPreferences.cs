namespace TrailerDownloader.Domain.Models;

public class UserPreferences
{
    public required Guid Id { get; init; }
    public required List<string> MediaDirectories { get; init; } = [];
    public required bool AutoDownload { get; init; }
    public required string TrailerLanguage { get; init; } = "en-US";
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}

namespace TrailerDownloader.Domain.Models;

public class UserPreferences
{
    public required Guid Id { get; init; }
    public required List<string> MediaDirectories { get; init; } = [];
    public required bool AutoDownload { get; init; }
    public required string TrailerLanguage { get; init; } = "en-US";
    public required List<string> VideoFileExtensions { get; init; } = [".mp4", ".mkv", ".avi"];
    public required string DownloadPath { get; init; } = "trailers";
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}

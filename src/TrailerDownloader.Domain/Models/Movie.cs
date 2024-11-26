namespace TrailerDownloader.Domain.Models;

public class Movie
{
    public required Guid Id { get; init; }
    public required string Title { get; init; }
    public required int Year { get; init; }
    public string? FilePath { get; init; }
    public string? PosterPath { get; init; }
    public string? TrailerUrl { get; init; }
    public bool TrailerExists { get; init; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}

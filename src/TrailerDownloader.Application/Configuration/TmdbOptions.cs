namespace TrailerDownloader.Application.Configuration;

public class TmdbOptions
{
    public const string ConfigSection = "Tmdb";
    public required string ApiKey { get; init; }
}

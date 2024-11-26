namespace TrailerDownloader.Infrastructure.Services;

using YoutubeExplode;
using YoutubeExplode.Converter;
using TrailerDownloader.Application.Interfaces;
using TrailerDownloader.Domain.Common;

public class YoutubeService : IYoutubeService
{
    private readonly YoutubeClient _youtube = new();

    public async Task<Result<string>> DownloadVideoAsync(string url, string outputPath)
    {
        try
        {
            await _youtube.Videos.DownloadAsync(url, outputPath,
                o => o.SetFFmpegPath("/usr/bin/ffmpeg"));

            return Result<string>.Success(outputPath);
        }
        catch (Exception ex)
        {
            return Result<string>.Failure($"Error downloading video: {ex.Message}");
        }
    }
}

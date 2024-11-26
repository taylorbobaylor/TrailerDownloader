namespace TrailerDownloader.Application.Interfaces;

using TrailerDownloader.Domain.Common;

public interface IYoutubeService
{
    Task<Result<string>> DownloadVideoAsync(string url, string outputPath);
}

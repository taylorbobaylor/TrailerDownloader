using TrailerDownloader.Models;

namespace TrailerDownloader.Services.Interfaces
{
    public interface IConfigService
    {
        Config GetConfig();
        bool SaveConfig(string path);
    }
}

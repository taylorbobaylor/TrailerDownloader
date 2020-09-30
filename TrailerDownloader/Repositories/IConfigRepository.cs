using TrailerDownloader.Models;

namespace TrailerDownloader.Repositories
{
    public interface IConfigRepository
    {
        Config GetConfig();
        bool SaveConfig(Config configs);
    }
}

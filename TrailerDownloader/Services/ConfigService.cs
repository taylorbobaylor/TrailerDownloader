using System.IO;
using TrailerDownloader.Models;
using TrailerDownloader.Repositories.Interfaces;
using TrailerDownloader.Services.Interfaces;

namespace TrailerDownloader.Services
{
    public class ConfigService : IConfigService
    {
        private readonly IConfigRepository _configRepo;

        public ConfigService(IConfigRepository configRepo)
        {
            _configRepo = configRepo;
        }

        public Config GetConfig()
        {
            return _configRepo.GetConfigAsync().Result;
        }

        public bool SaveConfig(string path)
        {
            if (!Directory.Exists(path))
            {
                return false;
            }

            return _configRepo.SaveConfigAsync(path).Result == 1;
        }
    }
}

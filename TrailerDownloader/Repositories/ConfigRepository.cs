using Newtonsoft.Json;
using System.IO;
using TrailerDownloader.Models;
using TrailerDownloader.Services;

namespace TrailerDownloader.Repositories
{
    public class ConfigRepository : IConfigRepository
    {
        private readonly IFileIOService _fileIOService;
        private static readonly string _configPath = Path.Combine(Directory.GetCurrentDirectory(), "config.json");

        public ConfigRepository(IFileIOService fileIOService)
        {
            _fileIOService = fileIOService;
        }

        public Config GetConfig()
        {
            if (_fileIOService.Exists(_configPath))
            {
                string json = _fileIOService.ReadAllText(_configPath);
                return JsonConvert.DeserializeObject<Config>(json);
            }

            return null;
        }

        public bool SaveConfig(Config configs)
        {
            if (Directory.Exists(configs.MediaDirectory) == false)
            {
                return false;
            }

            _fileIOService.WriteAllText(_configPath, JsonConvert.SerializeObject(configs));
            return true;
        }
    }
}

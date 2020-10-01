using Newtonsoft.Json;
using System.IO;
using TrailerDownloader.Models;

namespace TrailerDownloader.Repositories
{
    public class ConfigRepository : IConfigRepository
    {
        private static readonly string _configPath = Path.Combine(Directory.GetCurrentDirectory(), "config.json");

        public Config GetConfig()
        {
            if (File.Exists(_configPath))
            {
                string json = File.ReadAllText(_configPath);
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

            File.WriteAllText(_configPath, JsonConvert.SerializeObject(configs));
            return true;
        }
    }
}

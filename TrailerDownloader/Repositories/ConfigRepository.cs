using System.IO;
using System.Text.Json;
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
                return JsonSerializer.Deserialize<Config>(json);
            }

            return null;
        }

        public bool SaveConfig(Config configs)
        {
            if (Directory.Exists(configs.MediaDirectory) == false)
            {
                return false;
            }

            string json = JsonSerializer.Serialize(configs, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_configPath, json);
            return true;
        }
    }
}

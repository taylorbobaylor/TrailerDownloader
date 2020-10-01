using Newtonsoft.Json;
using System.IO;
using TrailerDownloader.Models;

namespace TrailerDownloader.Repositories
{
    public class ConfigRepository : IConfigRepository
    {
        private static readonly string configPath = Path.Combine(Directory.GetCurrentDirectory(), "config.json");

        public Config GetConfig()
        {
            if (File.Exists(configPath))
            {
                string json = File.ReadAllText(configPath);
                return JsonConvert.DeserializeObject<Config>(json);
            }

            return null;
        }

        public bool SaveConfig(Config configs)
        {
            File.WriteAllText(configPath, JsonConvert.SerializeObject(configs));
            return true;
        }
    }
}

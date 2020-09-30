using Newtonsoft.Json;
using System.IO;
using TrailerDownloader.Models;

namespace TrailerDownloader.Repositories
{
    public class ConfigRepository : IConfigRepository
    {
        public Config GetConfig()
        {
            if (File.Exists($@"{Directory.GetCurrentDirectory()}\config.json"))
            {
                string json = File.ReadAllText($@"{Directory.GetCurrentDirectory()}\config.json");
                return JsonConvert.DeserializeObject<Config>(json);
            }

            return null;
        }

        public bool SaveConfig(Config configs)
        {
            string path = $@"{Directory.GetCurrentDirectory()}\config.json";
            File.WriteAllText(path, JsonConvert.SerializeObject(configs));
            return true;
        }
    }
}

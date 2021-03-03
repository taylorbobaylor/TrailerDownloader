using System.Threading.Tasks;
using TrailerDownloader.Models;

namespace TrailerDownloader.Repositories.Interfaces
{
    public interface IConfigRepository
    {
        Task<Config> GetConfigAsync();
        Task<int> SaveConfigAsync(string path);
    }
}

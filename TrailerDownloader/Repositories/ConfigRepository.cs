using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TrailerDownloader.Context;
using TrailerDownloader.Models;
using TrailerDownloader.Repositories.Interfaces;

namespace TrailerDownloader.Repositories
{
    public class ConfigRepository : IConfigRepository
    {
        private readonly MovieDbContext _context;

        public ConfigRepository(MovieDbContext context)
        {
            _context = context;
        }

        public async Task<Config> GetConfigAsync()
        {
            return await _context.Config.FirstOrDefaultAsync();
        }

        public async Task<int> SaveConfigAsync(string path)
        {
            _context.Database.ExecuteSqlRaw("DELETE FROM CONFIG; DELETE FROM MOVIE");
            await _context.Config.AddAsync(new Config { BaseMediaPath = path });
            return await _context.SaveChangesAsync();
        }
    }
}

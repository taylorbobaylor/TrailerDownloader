using Microsoft.EntityFrameworkCore;
using TrailerDownloader.Models;

namespace TrailerDownloader.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<Config> Configs { get; set; }
    }
}

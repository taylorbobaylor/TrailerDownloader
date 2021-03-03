using Microsoft.EntityFrameworkCore;
using TrailerDownloader.Models;

namespace TrailerDownloader.Context
{
    public class MovieDbContext : DbContext
    {
        public MovieDbContext(DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<Config> Config { get; set; }
        public DbSet<Movie> Movie { get; set; }
    }
}

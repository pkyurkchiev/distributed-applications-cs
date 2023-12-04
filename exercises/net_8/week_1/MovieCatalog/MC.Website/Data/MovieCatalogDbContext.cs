using MC.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace MC.Website.Data
{
    public class MovieCatalogDbContext : DbContext
    {
        public DbSet<Movie> Movies { get; set; }
        public MovieCatalogDbContext(DbContextOptions<MovieCatalogDbContext> options) : base(options)
        {
        }
    }
}

using MC.Data.Entities;
using System.Data.Entity;

namespace MC.Data.Context
{
    public class MovieCatalogDbContext : DbContext
    {
        public DbSet<Movie> Movies { get; set; }
    }
}

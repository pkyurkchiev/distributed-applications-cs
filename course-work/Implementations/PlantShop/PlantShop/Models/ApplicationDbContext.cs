using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace PlantShop.Models
{
    public class ApplicationDbContext : IdentityDbContext<Users>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Soil> Soils { get; set; }
        public DbSet<Fertilizer> Fertilizers { get; set; }
        public DbSet<Plant> Plants { get; set; }
    }
}

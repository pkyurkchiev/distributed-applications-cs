using Microsoft.EntityFrameworkCore;
using RaceTrack.Models;

namespace RaceTrack.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Document> Documents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed данни - Администратор
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    UserId = 1,
                    FullName = "Kristiqn Admin",
                    Email = "admin@racetrack.com",
                    Username = "Kristiqn",
                    Password = "7a528c12", // В реален проект трябва да е хеширана!
                    PhoneNumber = "0888123456", // ✅ ПОПРАВЕНО: Phone → PhoneNumber
                    CreatedAt = DateTime.Now,
                    UserLevel = 2 // Администратор
                }
            );
        }
    }
}
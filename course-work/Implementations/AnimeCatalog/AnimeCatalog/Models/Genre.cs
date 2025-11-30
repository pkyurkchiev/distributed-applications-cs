using System.ComponentModel.DataAnnotations;

namespace AnimeCatalog.Models
{
    public class Genre
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(200)]
        public string Description { get; set; }

        public DateTime CreatedDate { get; set; }

        public bool IsAgeRestricted { get; set; }

        [Range(1, 10)]
        public double PopularityRating { get; set; }

        public int AnimeCount { get; set; } // Cache field

        public ICollection<Anime> Animes { get; set; }
    }
}

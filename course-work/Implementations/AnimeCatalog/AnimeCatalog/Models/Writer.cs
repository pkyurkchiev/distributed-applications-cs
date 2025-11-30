using System.ComponentModel.DataAnnotations;

namespace AnimeCatalog.Models
{
    public class Writer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } // Type 1: string

        // --- НОВОТО ПОЛЕ (ОПЦИОНАЛНО) ---
        [Display(Name = "Photo URL")]
        public string? ImageUrl { get; set; }
        // --------------------------------

        [Required]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; } // Type 2: DateTime

        [Range(0, 100)]
        public int YearsActive { get; set; } // Type 3: int

        public double NetWorthMillions { get; set; } // Type 4: double

        public bool IsRetired { get; set; } // Type 5: bool

        [StringLength(500)]
        public string Biography { get; set; }

        // Vrŭzka s Anime
        public ICollection<Anime> Animes { get; set; }
    }
}
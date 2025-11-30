using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnimeCatalog.Models
{
    public class Anime
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Title { get; set; }

        // --- НОВОТО ПОЛЕ (ОПЦИОНАЛНО) ---
        [Display(Name = "Image URL")]
        public string? ImageUrl { get; set; }
        // --------------------------------

        [StringLength(1000)]
        public string Description { get; set; }

        [DataType(DataType.Date)]
        public DateTime ReleaseDate { get; set; }

        [Range(0, 10)]
        public double IMDBRating { get; set; }

        public int Episodes { get; set; }

        public bool IsFinished { get; set; }

        // Foreign Keys
        [Display(Name = "Writer")]
        public int WriterId { get; set; }
        [ForeignKey("WriterId")]
        public Writer? Writer { get; set; }

        [Display(Name = "Genre")]
        public int GenreId { get; set; }
        [ForeignKey("GenreId")]
        public Genre? Genre { get; set; }
    }
}
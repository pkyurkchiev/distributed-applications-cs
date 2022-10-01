using System.ComponentModel.DataAnnotations;

namespace MC.Data.Entities
{
    public class Movie
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(150, MinimumLength = 3)]
        public string Title { get; set; }
        [DataType(dataType: DataType.Date)]
        public DateTime ReleaseDate { get; set; }

        public int GenreId { get; set; }
        public virtual Genre Genre { get; set; }

        public int? Rating { get; set; }
        [StringLength(100, MinimumLength = 3)]
        public string Country { get; set; }
    }
}
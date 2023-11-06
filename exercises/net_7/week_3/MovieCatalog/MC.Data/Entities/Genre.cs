using System.ComponentModel.DataAnnotations;

namespace MC.Data.Entities
{
    public class Genre
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Value { get; set; }

        public virtual ICollection<Movie>? Movies { get; set; }
    }
}

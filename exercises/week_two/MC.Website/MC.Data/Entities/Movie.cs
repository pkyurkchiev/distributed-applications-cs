using System;
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

        [Display(Name = "Release date")]
        [DataType(DataType.Date)]
        public DateTime? ReleaseDate { get; set; }

        [StringLength(300, MinimumLength = 2)]
        public string Country { get; set; }

        [StringLength(4000)]
        public string Description { get; set; }

        [Display(Name = "Rating")]
        public int? RatingId { get; set; }
        public virtual Rating Rating { get; set; }

        [Display(Name = "Writer")]
        public int? WriterId { get; set; }
        public virtual Writer Writer { get; set; }
    }
}

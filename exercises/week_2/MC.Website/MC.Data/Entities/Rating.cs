using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC.Data.Entities
{
    public class Rating
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Rating")]
        [Required]
        [StringLength(5)]
        public string RatingValue { get; set; }

        public virtual ICollection<Movie> Movies { get; set; }
    }
}

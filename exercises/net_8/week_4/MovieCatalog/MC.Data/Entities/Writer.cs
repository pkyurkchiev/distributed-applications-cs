using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC.Data.Entities
{
    public class Writer
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "First name")]
        [Required]
        [StringLength(150, MinimumLength = 3)]
        public string FirstName { get; set; }

        [Display(Name = "Last name")]
        [StringLength(150, MinimumLength = 3)]
        public string LastName { get; set; }

        [Display(Name = "Writer username")]
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string UserName { get; set; }

        public virtual ICollection<Movie>? Movies { get; set; }
    }
}

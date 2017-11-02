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

        [Display(Name = "First Name")]
        [Required]
        [StringLength(150, MinimumLength = 3)]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [Required]
        [StringLength(150, MinimumLength = 3)]
        public string LastName { get; set; }

        [Display(Name = "Username")]
        [Required]
        [StringLength(150, MinimumLength = 3)]
        public string UserName { get; set; }

        public virtual ICollection<Movie> Movies { get; set; }
    }
}

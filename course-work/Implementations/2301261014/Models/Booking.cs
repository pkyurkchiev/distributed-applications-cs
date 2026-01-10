using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceTrack.Models
{
    public class Booking
    {
        [Key]
        public int BookingId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Планът е задължителен")]
        [StringLength(50)]
        public string Plan { get; set; } // "Начинаещ", "Напредващ", "Професионален"

        [Required(ErrorMessage = "Името е задължително")]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email адресът е задължителен")]
        [StringLength(150)]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Телефонът е задължителен")]
        [StringLength(15)]
        public string Phone { get; set; }

        [StringLength(200)]
        public string Address { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }

        public DateTime BookingDate { get; set; }

        [StringLength(50)]
        public string Status { get; set; } = "Pending"; // "Pending", "Confirmed", "Completed", "Cancelled"

        // ✅ ДОБАВИХ ТОВА СВОЙСТВО
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Връзки
        [ForeignKey("UserId")]
        public User User { get; set; }

        public ICollection<Document> Documents { get; set; } = new List<Document>();
    }
}
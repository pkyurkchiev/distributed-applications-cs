using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceTrack.Models
{
    public class Document
    {
        [Key]
        public int DocumentId { get; set; }

        [Required]
        public int BookingId { get; set; }

        [Required]
        [StringLength(200)]
        public string FileName { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string FilePath { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string DocumentType { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Pending";

        [Required]
        public DateTime UploadDate { get; set; } = DateTime.Now;

        // ДОБАВЕНИ СВОЙСТВА за BookingController
        public long FileSize { get; set; }

        public DateTime UploadedAt { get; set; } = DateTime.Now;

        // Navigation property
        [ForeignKey("BookingId")]
        public virtual Booking Booking { get; set; } = null!;
    }
}
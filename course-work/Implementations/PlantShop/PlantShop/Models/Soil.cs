using System;
using System.ComponentModel.DataAnnotations;

namespace PlantShop.Models
{
    public class Soil
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Range(0, 14)]
        public double PH { get; set; }

        [Range(0.1, 1000)]
        public double BagWeightKg { get; set; }

      

        [DataType(DataType.Currency)]
        [Range(0.01, 10000)]
        public decimal PricePerBag { get; set; }

        [Range(0, int.MaxValue)]
        public int InStockQuantity { get; set; }

        public bool IsOrganic { get; set; }

        [DataType(DataType.Date)]
        public DateTime CreatedOn { get; set; } = DateTime.Now;

        [MaxLength(2000, ErrorMessage = "The description is too long.")]
        public string Description { get; set; }

    }
}

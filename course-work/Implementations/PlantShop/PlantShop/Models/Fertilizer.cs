    using System;
using System.ComponentModel.DataAnnotations;

namespace PlantShop.Models
{
    public class Fertilizer
    {

        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(20)]
        public string NPKRatio { get; set; }

        [DataType(DataType.Currency)]
        [Range(0.01, 10000)]
        public decimal PricePerKg { get; set; }

        [Range(0, 50000)]
        public int InStockQuantity { get; set; }

        public bool IsLiquid { get; set; }

        [DataType(DataType.Date)]
        public DateTime CreatedOn { get; set; } = DateTime.Now;

        [MaxLength(2000)]
        public string Description { get; set; }

    }
}

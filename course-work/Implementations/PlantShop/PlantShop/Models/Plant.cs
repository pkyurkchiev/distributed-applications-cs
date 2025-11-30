using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace PlantShop.Models
{
    public class Plant
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(100)]
        public string Species { get; set; }


        [DataType(DataType.Currency)]
        [Range(0.01, 10000)]
        public decimal Price { get; set; }

        [Range(0, 10000)]
        public int StockQuantity { get; set; }

        public bool IsIndoor { get; set; }

        [DataType(DataType.Date)]
        public DateTime? PlantedOn { get; set; }

        [MaxLength(2000)]
        public string Description { get; set; }

        // FK към Soil
        [Required]
        public int SoilId { get; set; }

        [ValidateNever]
        public Soil Soil { get; set; }

        // FK към Fertilizer
        [Required]
        public int FertilizerId { get; set; }

        
        [ValidateNever]
        public Fertilizer Fertilizer { get; set; }
    }
}

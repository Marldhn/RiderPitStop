using RiderPitStop.Models;
using System.ComponentModel.DataAnnotations;

namespace RiderPitStop.Models
{
    public class Product
    {
        public enum Category
        {
            Tire,
            Accessory
        }

        [Key]
        [Required]
        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public string Description { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? SRP { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? DP { get; set; }

        public string Type { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Stock Quantity must be a positive number.")]
        public int? StockQuantity { get; set; }

        public string Brand { get; set; }

        // Use the enum type for ProductCategory
        [Required]
        public Category ProductCategory { get; set; }

        public virtual ICollection<RequestOrder> RequestOrders { get; set; }
    }
}

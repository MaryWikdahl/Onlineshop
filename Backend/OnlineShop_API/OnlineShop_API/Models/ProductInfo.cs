using System.ComponentModel.DataAnnotations;

namespace OnlineShop_API.Models
{
    public class ProductInfo
    {
        [Key]
        [Required]
        public int Id { get; set; }
        [Required]
        public int ProductId { get; set; }
        public Products Product { get; set; }
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity must be a non-negative integer.")]
        public int StockQuantity { get; set; }
        public string Size { get; set; }
        public string Color { get; set; }
    }
}

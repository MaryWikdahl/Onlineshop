using System.ComponentModel.DataAnnotations;

namespace OnlineShop_API.DTOs
{
    public class ProductInfoDto
    {
        public int StockQuantity { get; set; }
        public string Size { get; set; }
        public string Color { get; set; }
    }
}

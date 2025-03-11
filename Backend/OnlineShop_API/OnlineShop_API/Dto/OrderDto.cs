using OnlineShop_API.Dto;
using System.ComponentModel.DataAnnotations;

namespace OnlineShop_API.Dto
{
    public class OrderDto
{
    public int Id { get; set; }
        public string Status { get; set; }
        public DateTime OrderDate { get; set; }
    // Lägg till andra egenskaper som du behöver
    public List<OrderItemDto> OrderItems { get; set; }
}
}


using OnlineShop.Models;

namespace OnlineShop.Services;

public class CartItem
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public bool IsOutOfStock { get; set; }
    public string? ImageUrl { get; set; }
}



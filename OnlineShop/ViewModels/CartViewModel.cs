using OnlineShop.Services;

namespace OnlineShop.ViewModels;

public class CartViewModel
{
    public List<CartItem> Items { get; set; } = new();
    public decimal Total { get; set; }
}



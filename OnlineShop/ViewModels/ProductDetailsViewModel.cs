using OnlineShop.Models;

namespace OnlineShop.ViewModels;

public class ProductDetailsViewModel
{
    public Product Product { get; set; } = null!;
    public ProductInventory? Inventory { get; set; }
    public IEnumerable<ProductImage> Images { get; set; } = Enumerable.Empty<ProductImage>();
    public IEnumerable<ProductReview> Reviews { get; set; } = Enumerable.Empty<ProductReview>();
    public ProductPromo? ActivePromo { get; set; }

    public bool IsOutOfStock => Inventory == null || Inventory.StockQuantity <= 0;

    public decimal EffectivePrice => ActivePromo != null
        ? Math.Max(0, Product.Price - ActivePromo.AmountOff)
        : Product.Price;
}



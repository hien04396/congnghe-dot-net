namespace OnlineShop.Models;

public class ProductInventory
{
    public int Id { get; set; }

    public int ProductId { get; set; }
    public Product? Product { get; set; }

    public int StockQuantity { get; set; }

    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}



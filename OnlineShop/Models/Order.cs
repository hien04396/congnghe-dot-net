using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShop.Models;

public class Order
{
    public int Id { get; set; }

    public int CustomerUserId { get; set; }
    public CustomerUser? Customer { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }

    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}



using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Models;

public class ProductReview
{
    public int Id { get; set; }

    public int ProductId { get; set; }
    public Product? Product { get; set; }

    [MaxLength(200)]
    public string? Title { get; set; }

    public string? Content { get; set; }

    [Range(1, 5)]
    public int Rating { get; set; }

    public int? CreatedByAdminId { get; set; }
    public AdminUser? CreatedByAdmin { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}



using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Models;

public class AdminUser
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Password { get; set; } = string.Empty;

    public bool IsDefault { get; set; }

    public ICollection<ProductReview> Reviews { get; set; } = new List<ProductReview>();
}



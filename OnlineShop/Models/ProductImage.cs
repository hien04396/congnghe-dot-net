using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Models;

public class ProductImage
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public Product? Product { get; set; }

    [Required(ErrorMessage = "Image URL is required")]
    [Url(ErrorMessage = "Please enter a valid URL")]
    public string ImageUrl { get; set; } = string.Empty;

    public bool IsPrimary { get; set; }
}



using OnlineShop.Models;

namespace OnlineShop.ViewModels;

public class ProductListViewModel
{
    public IEnumerable<ProductCategory> Categories { get; set; } = Enumerable.Empty<ProductCategory>();
    public IEnumerable<Product> Products { get; set; } = Enumerable.Empty<Product>();

    public int? SelectedCategoryId { get; set; }
    public string? Search { get; set; }

    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }

    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}



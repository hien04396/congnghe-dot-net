using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Data;
using OnlineShop.ViewModels;

namespace OnlineShop.Controllers;

public class StoreController : Controller
{
    private readonly OnlineStoreContext _context;

    public StoreController(OnlineStoreContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Categories()
    {
        var categories = await _context.ProductCategories
            .OrderBy(c => c.Name)
            .ToListAsync();
        return View(categories);
    }

    public async Task<IActionResult> Products(int? categoryId, string? search, int page = 1, int pageSize = 8)
    {
        var categories = await _context.ProductCategories.OrderBy(c => c.Name).ToListAsync();

        var query = _context.Products
            .Include(p => p.Category)
            .Include(p => p.Inventory)
            .Where(p => p.IsActive)
            .AsQueryable();

        if (categoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == categoryId.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(p => p.Name.Contains(search));
        }

        var totalCount = await query.CountAsync();
        var products = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Include(p => p.Images)
            .ToListAsync();

        var vm = new ProductListViewModel
        {
            Categories = categories,
            Products = products,
            SelectedCategoryId = categoryId,
            Search = search,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };

        return View(vm);
    }

    public async Task<IActionResult> Details(int id)
    {
        var product = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Images)
            .Include(p => p.Inventory)
            .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);

        if (product == null)
        {
            return NotFound();
        }

        var reviews = await _context.ProductReviews
            .Where(r => r.ProductId == id)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        var promo = await _context.ProductPromos
            .Where(pr => pr.ProductId == id && pr.IsActive)
            .OrderByDescending(pr => pr.StartDate)
            .FirstOrDefaultAsync();

        var vm = new ProductDetailsViewModel
        {
            Product = product,
            Inventory = product.Inventory,
            Images = product.Images,
            Reviews = reviews,
            ActivePromo = promo
        };

        return View(vm);
    }
}



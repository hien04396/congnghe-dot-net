using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Data;

namespace OnlineShop.Controllers;

[Authorize(Roles = "Admin", AuthenticationSchemes = "AdminScheme")]
public class AdminDashboardController : Controller
{
    private readonly OnlineStoreContext _context;

    public AdminDashboardController(OnlineStoreContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var productCount = await _context.Products.CountAsync();
        var categoryCount = await _context.ProductCategories.CountAsync();
        var orderCount = await _context.Orders.CountAsync();
        var lowStockCount = await _context.ProductInventories.CountAsync(i => i.StockQuantity <= 3);

        ViewData["ProductCount"] = productCount;
        ViewData["CategoryCount"] = categoryCount;
        ViewData["OrderCount"] = orderCount;
        ViewData["LowStockCount"] = lowStockCount;

        return View();
    }
}



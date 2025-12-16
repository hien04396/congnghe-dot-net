using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Data;

namespace OnlineShop.Controllers;

[Authorize(Roles = "Admin")]
public class AdminProductInventoryController : Controller
{
    private readonly OnlineStoreContext _context;

    public AdminProductInventoryController(OnlineStoreContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var items = await _context.ProductInventories
            .Include(i => i.Product)
            .OrderBy(i => i.Product!.Name)
            .ToListAsync();
        return View(items);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(int id, int stockQuantity)
    {
        var inventory = await _context.ProductInventories.FindAsync(id);
        if (inventory == null)
        {
            return NotFound();
        }

        inventory.StockQuantity = stockQuantity;
        inventory.LastUpdated = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}



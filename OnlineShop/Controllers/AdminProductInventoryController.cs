using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Data;
using OnlineShop.Models;

namespace OnlineShop.Controllers;

[Authorize(Roles = "Admin", AuthenticationSchemes = "AdminScheme")]
public class AdminProductInventoryController : Controller
{
    private readonly OnlineStoreContext _context;

    public AdminProductInventoryController(OnlineStoreContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> ManageStock(int productId)
    {
        var product = await _context.Products.FindAsync(productId);
        if (product == null)
        {
            return NotFound();
        }

        // Check if inventory exists for this product
        var inventory = await _context.ProductInventories
            .Include(i => i.Product)
            .FirstOrDefaultAsync(i => i.ProductId == productId);

        if (inventory == null)
        {
            // Create inventory record if it doesn't exist
            inventory = new ProductInventory
            {
                ProductId = productId,
                StockQuantity = 0,
                LastUpdated = DateTime.UtcNow
            };
            _context.ProductInventories.Add(inventory);
            await _context.SaveChangesAsync();

            // Reload with product included
            inventory = await _context.ProductInventories
                .Include(i => i.Product)
                .FirstOrDefaultAsync(i => i.ProductId == productId);
        }

        return View("ManageStock", inventory);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ManageStock(int id, int stockQuantity, int adjustment = 0)
    {
        var inventory = await _context.ProductInventories
            .Include(i => i.Product)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (inventory == null)
        {
            return NotFound();
        }

        int newQuantity;
        if (adjustment != 0)
        {
            newQuantity = inventory.StockQuantity + adjustment;
        }
        else
        {
            newQuantity = stockQuantity;
        }

        if (newQuantity < 0)
        {
            TempData["Error"] = "Stock quantity cannot be negative.";
            return View(inventory);
        }

        inventory.StockQuantity = newQuantity;
        inventory.LastUpdated = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        TempData["Success"] = $"Stock updated successfully! New quantity: {newQuantity}";
        return View(inventory);
    }
}



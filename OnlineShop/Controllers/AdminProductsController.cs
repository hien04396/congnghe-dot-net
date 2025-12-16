using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Data;
using OnlineShop.Models;

namespace OnlineShop.Controllers;

[Authorize(Roles = "Admin")]
public class AdminProductsController : Controller
{
    private readonly OnlineStoreContext _context;

    public AdminProductsController(OnlineStoreContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(int? categoryId = null)
    {
        ViewData["Categories"] = new SelectList(await _context.ProductCategories.OrderBy(c => c.Name).ToListAsync(), "Id", "Name");

        var query = _context.Products
            .Include(p => p.Category)
            .Include(p => p.Inventory)
            .AsQueryable();

        if (categoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == categoryId.Value);
            ViewData["SelectedCategoryId"] = categoryId.Value;
        }

        var products = await query
            .OrderBy(p => p.Name)
            .ToListAsync();

        return View(products);
    }

    public async Task<IActionResult> Create()
    {
        await PopulateCategoriesDropDownList();
        return View(new Product());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Product product)
    {
        if (!ModelState.IsValid)
        {
            await PopulateCategoriesDropDownList(product.CategoryId);
            return View(product);
        }

        product.CreatedAt = DateTime.UtcNow;
        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        // create empty inventory
        _context.ProductInventories.Add(new ProductInventory
        {
            ProductId = product.Id,
            StockQuantity = 0
        });
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null)
        {
            return NotFound();
        }

        await PopulateCategoriesDropDownList(product.CategoryId);
        return View(product);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Product product)
    {
        if (id != product.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            await PopulateCategoriesDropDownList(product.CategoryId);
            return View(product);
        }

        _context.Update(product);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var product = await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);
        if (product == null)
        {
            return NotFound();
        }
        return View(product);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product != null)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateCategoriesDropDownList(object? selectedCategory = null)
    {
        var categories = await _context.ProductCategories
            .OrderBy(c => c.Name)
            .ToListAsync();
        ViewBag.CategoryId = new SelectList(categories, "Id", "Name", selectedCategory);
    }
}



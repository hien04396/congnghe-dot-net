using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Data;
using OnlineShop.Models;

namespace OnlineShop.Controllers;

[Authorize(Roles = "Admin", AuthenticationSchemes = "AdminScheme")]
public class AdminProductCategoriesController : Controller
{
    private readonly OnlineStoreContext _context;

    public AdminProductCategoriesController(OnlineStoreContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(int page = 1)
    {
        const int pageSize = 15;

        var query = _context.ProductCategories
            .OrderByDescending(c => c.CreatedAt);

        // Get total count for pagination
        var totalItems = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
        
        // Ensure page is within valid range
        page = Math.Max(1, Math.Min(page, Math.Max(1, totalPages)));

        var categories = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        // Pass pagination info to view
        ViewData["CurrentPage"] = page;
        ViewData["TotalPages"] = totalPages;
        ViewData["TotalItems"] = totalItems;

        return View(categories);
    }

    public IActionResult Create()
    {
        return View(new ProductCategory());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductCategory category)
    {
        if (!ModelState.IsValid)
        {
            return View(category);
        }

        category.CreatedAt = DateTime.UtcNow;
        _context.ProductCategories.Add(category);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var category = await _context.ProductCategories.FindAsync(id);
        if (category == null)
        {
            return NotFound();
        }
        return View(category);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ProductCategory category)
    {
        if (id != category.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(category);
        }

        _context.Update(category);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var category = await _context.ProductCategories.FindAsync(id);
        if (category == null)
        {
            return NotFound();
        }
        return View(category);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var category = await _context.ProductCategories.FindAsync(id);
        if (category != null)
        {
            _context.ProductCategories.Remove(category);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }
}



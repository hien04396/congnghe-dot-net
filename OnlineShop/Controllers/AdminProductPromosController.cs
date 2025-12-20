using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Data;
using OnlineShop.Models;

namespace OnlineShop.Controllers;

[Authorize(Roles = "Admin", AuthenticationSchemes = "AdminScheme")]
public class AdminProductPromosController : Controller
{
    private readonly OnlineStoreContext _context;

    public AdminProductPromosController(OnlineStoreContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(int? productId = null)
    {
        var query = _context.ProductPromos
            .Include(p => p.Product)
            .AsQueryable();

        if (productId.HasValue)
        {
            query = query.Where(p => p.ProductId == productId.Value);
            ViewData["ProductId"] = productId.Value;
            
            var product = await _context.Products.FindAsync(productId.Value);
            ViewData["ProductName"] = product?.Name;
        }

        var promos = await query
            .OrderByDescending(p => p.IsActive)
            .ThenBy(p => p.Product!.Name)
            .ToListAsync();

        ViewData["Products"] = await _context.Products
            .OrderBy(p => p.Name)
            .ToListAsync();

        return View(promos);
    }

    public async Task<IActionResult> Create(int? productId = null)
    {
        ViewData["Products"] = await _context.Products
            .OrderBy(p => p.Name)
            .ToListAsync();
        
        var promo = new ProductPromo
        {
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(7),
            IsActive = true
        };
        
        if (productId.HasValue)
        {
            promo.ProductId = productId.Value;
        }
        
        return View(promo);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductPromo promo, int? returnProductId)
    {
        if (!ModelState.IsValid)
        {
            ViewData["Products"] = await _context.Products
                .OrderBy(p => p.Name)
                .ToListAsync();
            return View(promo);
        }

        if (promo.IsActive)
        {
            var existing = await _context.ProductPromos
                .Where(p => p.ProductId == promo.ProductId && p.IsActive)
                .ToListAsync();
            foreach (var p in existing)
            {
                p.IsActive = false;
            }
        }

        _context.ProductPromos.Add(promo);
        await _context.SaveChangesAsync();
        
        if (returnProductId.HasValue)
        {
            return RedirectToAction(nameof(Index), new { productId = returnProductId });
        }
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id, int? productId = null)
    {
        var promo = await _context.ProductPromos.FindAsync(id);
        if (promo == null)
        {
            return NotFound();
        }

        ViewData["Products"] = await _context.Products
            .OrderBy(p => p.Name)
            .ToListAsync();
        return View(promo);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ProductPromo promo, int? returnProductId)
    {
        if (id != promo.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            ViewData["Products"] = await _context.Products
                .OrderBy(p => p.Name)
                .ToListAsync();
            return View(promo);
        }

        if (promo.IsActive)
        {
            var existing = await _context.ProductPromos
                .Where(p => p.ProductId == promo.ProductId && p.IsActive && p.Id != promo.Id)
                .ToListAsync();
            foreach (var p in existing)
            {
                p.IsActive = false;
            }
        }

        _context.Update(promo);
        await _context.SaveChangesAsync();
        
        if (returnProductId.HasValue)
        {
            return RedirectToAction(nameof(Index), new { productId = returnProductId });
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, int? returnProductId)
    {
        var promo = await _context.ProductPromos.FindAsync(id);
        if (promo == null)
        {
            return NotFound();
        }

        var productId = promo.ProductId;
        _context.ProductPromos.Remove(promo);
        await _context.SaveChangesAsync();
        
        if (returnProductId.HasValue)
        {
            return RedirectToAction(nameof(Index), new { productId = returnProductId });
        }
        return RedirectToAction(nameof(Index), new { productId });
    }
}



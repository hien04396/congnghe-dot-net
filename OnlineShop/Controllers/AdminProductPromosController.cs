using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Data;
using OnlineShop.Models;

namespace OnlineShop.Controllers;

[Authorize(Roles = "Admin")]
public class AdminProductPromosController : Controller
{
    private readonly OnlineStoreContext _context;

    public AdminProductPromosController(OnlineStoreContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var promos = await _context.ProductPromos
            .Include(p => p.Product)
            .OrderByDescending(p => p.IsActive)
            .ThenBy(p => p.Product!.Name)
            .ToListAsync();
        return View(promos);
    }

    public async Task<IActionResult> Create()
    {
        ViewData["Products"] = await _context.Products
            .OrderBy(p => p.Name)
            .ToListAsync();
        return View(new ProductPromo
        {
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(7),
            IsActive = true
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductPromo promo)
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
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
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
    public async Task<IActionResult> Edit(int id, ProductPromo promo)
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
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var promo = await _context.ProductPromos.FindAsync(id);
        if (promo == null)
        {
            return NotFound();
        }

        _context.ProductPromos.Remove(promo);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}



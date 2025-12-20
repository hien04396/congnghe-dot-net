using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Data;
using OnlineShop.Models;

namespace OnlineShop.Controllers;

[Authorize(Roles = "Admin", AuthenticationSchemes = "AdminScheme")]
public class AdminProductImagesController : Controller
{
    private readonly OnlineStoreContext _context;

    public AdminProductImagesController(OnlineStoreContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(int productId)
    {
        var product = await _context.Products
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == productId);
        if (product == null)
        {
            return NotFound();
        }

        return View(product);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(int productId, string imageUrl, bool isPrimary = false)
    {
        var product = await _context.Products.FindAsync(productId);
        if (product == null)
        {
            return NotFound();
        }

        if (string.IsNullOrWhiteSpace(imageUrl))
        {
            TempData["Error"] = "Image URL is required.";
            return RedirectToAction(nameof(Index), new { productId });
        }

        // Validate URL format
        if (!Uri.TryCreate(imageUrl.Trim(), UriKind.Absolute, out var uriResult) ||
            (uriResult.Scheme != Uri.UriSchemeHttp && uriResult.Scheme != Uri.UriSchemeHttps))
        {
            TempData["Error"] = "Please enter a valid URL (must start with http:// or https://).";
            return RedirectToAction(nameof(Index), new { productId });
        }

        if (isPrimary)
        {
            var existing = await _context.ProductImages
                .Where(i => i.ProductId == productId && i.IsPrimary)
                .ToListAsync();
            foreach (var img in existing)
            {
                img.IsPrimary = false;
            }
        }

        _context.ProductImages.Add(new ProductImage
        {
            ProductId = productId,
            ImageUrl = imageUrl.Trim(),
            IsPrimary = isPrimary
        });
        await _context.SaveChangesAsync();
        
        TempData["Success"] = "Image added successfully.";
        return RedirectToAction(nameof(Index), new { productId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var image = await _context.ProductImages.FindAsync(id);
        if (image == null)
        {
            return NotFound();
        }

        var productId = image.ProductId;
        _context.ProductImages.Remove(image);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index), new { productId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MakePrimary(int id)
    {
        var image = await _context.ProductImages.FindAsync(id);
        if (image == null)
        {
            return NotFound();
        }

        var productId = image.ProductId;

        var existing = await _context.ProductImages
            .Where(i => i.ProductId == productId && i.IsPrimary)
            .ToListAsync();
        foreach (var img in existing)
        {
            img.IsPrimary = false;
        }

        image.IsPrimary = true;
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index), new { productId });
    }
}



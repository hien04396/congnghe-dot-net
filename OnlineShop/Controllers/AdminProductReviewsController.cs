using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Data;
using OnlineShop.Models;
using System.Security.Claims;

namespace OnlineShop.Controllers;

[Authorize(Roles = "Admin", AuthenticationSchemes = "AdminScheme")]
public class AdminProductReviewsController : Controller
{
    private readonly OnlineStoreContext _context;

    public AdminProductReviewsController(OnlineStoreContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(int? productId = null)
    {
        var query = _context.ProductReviews
            .Include(r => r.Product)
            .AsQueryable();

        if (productId.HasValue)
        {
            query = query.Where(r => r.ProductId == productId.Value);
            ViewData["ProductId"] = productId.Value;
        }

        var reviews = await query
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        ViewData["Products"] = await _context.Products
            .OrderBy(p => p.Name)
            .ToListAsync();

        return View(reviews);
    }

    public async Task<IActionResult> Create()
    {
        ViewData["Products"] = await _context.Products
            .OrderBy(p => p.Name)
            .ToListAsync();
        return View(new ProductReview { Rating = 5 });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductReview review)
    {
        if (!ModelState.IsValid)
        {
            ViewData["Products"] = await _context.Products
                .OrderBy(p => p.Name)
                .ToListAsync();
            return View(review);
        }

        if (int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var adminId))
        {
            review.CreatedByAdminId = adminId;
        }
        review.CreatedAt = DateTime.UtcNow;

        _context.ProductReviews.Add(review);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var review = await _context.ProductReviews.FindAsync(id);
        if (review == null)
        {
            return NotFound();
        }

        ViewData["Products"] = await _context.Products
            .OrderBy(p => p.Name)
            .ToListAsync();

        return View(review);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ProductReview review)
    {
        if (id != review.Id)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            ViewData["Products"] = await _context.Products
                .OrderBy(p => p.Name)
                .ToListAsync();
            return View(review);
        }

        _context.Update(review);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var review = await _context.ProductReviews.FindAsync(id);
        if (review == null)
        {
            return NotFound();
        }

        _context.ProductReviews.Remove(review);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}



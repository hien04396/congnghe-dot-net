using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Data;
using OnlineShop.Services;
using OnlineShop.ViewModels;
using System.Security.Claims;

namespace OnlineShop.Controllers;

public class CartController : Controller
{
    private readonly CartService _cartService;
    private readonly OnlineStoreContext _context;

    public CartController(CartService cartService, OnlineStoreContext context)
    {
        _cartService = cartService;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        await _cartService.RefreshStockFlagsAsync();
        var items = _cartService.GetCart();
        var vm = new CartViewModel
        {
            Items = items,
            Total = _cartService.GetTotal()
        };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(int productId, int quantity = 1)
    {
        await _cartService.AddToCartAsync(productId, quantity);
        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Update(int productId, int quantity)
    {
        _cartService.UpdateQuantity(productId, quantity);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Remove(int productId)
    {
        _cartService.RemoveItem(productId);
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Customer")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Checkout()
    {
        await _cartService.RefreshStockFlagsAsync();
        var items = _cartService.GetCart();

        // Remove out-of-stock items automatically
        items = items.Where(i => !i.IsOutOfStock && i.Quantity > 0).ToList();
        if (!items.Any())
        {
            TempData["Error"] = "Nothing to checkout. All items are out of stock.";
            _cartService.Clear();
            return RedirectToAction(nameof(Index));
        }

        var productIds = items.Select(i => i.ProductId).ToList();
        var inventories = await _context.ProductInventories
            .Where(i => productIds.Contains(i.ProductId))
            .ToDictionaryAsync(i => i.ProductId, i => i);

        foreach (var item in items)
        {
            if (!inventories.TryGetValue(item.ProductId, out var inv) || inv.StockQuantity < item.Quantity)
            {
                TempData["Error"] = "Some items no longer have enough stock.";
                return RedirectToAction(nameof(Index));
            }
        }

        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var order = new Models.Order
        {
            CustomerUserId = userId,
            CreatedAt = DateTime.UtcNow,
            TotalAmount = items.Sum(i => i.UnitPrice * i.Quantity),
            Items = items.Select(i => new Models.OrderItem
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                LineTotal = i.UnitPrice * i.Quantity
            }).ToList()
        };

        _context.Orders.Add(order);

        foreach (var item in items)
        {
            var inv = inventories[item.ProductId];
            inv.StockQuantity -= item.Quantity;
            inv.LastUpdated = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
        _cartService.Clear();

        TempData["Success"] = "Order placed successfully!";
        return RedirectToAction("Details", "Orders", new { id = order.Id });
    }
}



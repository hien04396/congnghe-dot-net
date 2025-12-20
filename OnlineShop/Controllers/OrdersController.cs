using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Data;
using System.Security.Claims;

namespace OnlineShop.Controllers;

[Authorize(Roles = "Customer", AuthenticationSchemes = "CustomerScheme")]
public class OrdersController : Controller
{
    private readonly OnlineStoreContext _context;

    public OrdersController(OnlineStoreContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var orders = await _context.Orders
            .Where(o => o.CustomerUserId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
        return View(orders);
    }

    public async Task<IActionResult> Details(int id)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var order = await _context.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.Id == id && o.CustomerUserId == userId);

        if (order == null)
        {
            return NotFound();
        }

        return View(order);
    }
}



using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Data;
using OnlineShop.Models;

namespace OnlineShop.Controllers;

[Authorize(Roles = "Admin", AuthenticationSchemes = "AdminScheme")]
public class AdminUsersController : Controller
{
    private readonly OnlineStoreContext _context;

    public AdminUsersController(OnlineStoreContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var admins = await _context.AdminUsers
            .OrderBy(a => a.Username)
            .ToListAsync();
        return View(admins);
    }

    public IActionResult Create()
    {
        return View(new AdminUser());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AdminUser admin)
    {
        if (!ModelState.IsValid)
        {
            return View(admin);
        }

        admin.IsDefault = false;
        _context.AdminUsers.Add(admin);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var admin = await _context.AdminUsers.FindAsync(id);
        if (admin == null)
        {
            return NotFound();
        }
        if (admin.IsDefault)
        {
            TempData["Error"] = "Default admin cannot be edited here.";
            return RedirectToAction(nameof(Index));
        }
        return View(admin);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, AdminUser admin)
    {
        if (id != admin.Id)
        {
            return NotFound();
        }

        var existing = await _context.AdminUsers.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);
        if (existing == null || existing.IsDefault)
        {
            TempData["Error"] = "Default admin cannot be edited.";
            return RedirectToAction(nameof(Index));
        }

        admin.IsDefault = false;
        if (!ModelState.IsValid)
        {
            return View(admin);
        }

        _context.Update(admin);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var admin = await _context.AdminUsers.FindAsync(id);
        if (admin == null)
        {
            return NotFound();
        }
        if (admin.IsDefault)
        {
            TempData["Error"] = "Default admin cannot be deleted.";
            return RedirectToAction(nameof(Index));
        }
        return View(admin);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var admin = await _context.AdminUsers.FindAsync(id);
        if (admin != null && !admin.IsDefault)
        {
            _context.AdminUsers.Remove(admin);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}



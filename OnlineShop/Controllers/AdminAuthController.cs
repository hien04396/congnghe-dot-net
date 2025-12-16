using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Data;
using OnlineShop.ViewModels;

namespace OnlineShop.Controllers;

public class AdminAuthController : Controller
{
    private readonly OnlineStoreContext _context;

    public AdminAuthController(OnlineStoreContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Route("admin")]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.IsInRole("Admin"))
        {
            return RedirectToAction("Index", "AdminDashboard");
        }

        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("admin")]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var username = model.Username.Trim();
        var password = model.Password;

        var isDefaultAdmin = username == "admin" && password == "admin";

        var admin = isDefaultAdmin
            ? await _context.AdminUsers.FirstOrDefaultAsync(a => a.Username == "admin")
            : await _context.AdminUsers.FirstOrDefaultAsync(a => a.Username == username && a.Password == password);

        if (!isDefaultAdmin && admin == null)
        {
            ModelState.AddModelError(string.Empty, "Invalid username or password.");
            return View(model);
        }

        // If default admin and not in DB yet, create it
        if (isDefaultAdmin && admin == null)
        {
            admin = new Models.AdminUser
            {
                Username = "admin",
                Password = "admin",
                IsDefault = true
            };
            _context.AdminUsers.Add(admin);
            await _context.SaveChangesAsync();
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, admin!.Id.ToString()),
            new Claim(ClaimTypes.Name, admin.Username),
            new Claim(ClaimTypes.Role, "Admin")
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
        {
            return Redirect(model.ReturnUrl);
        }

        return RedirectToAction("Index", "AdminDashboard");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }
}



using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Data;
using OnlineShop.Models;
using OnlineShop.ViewModels;

namespace OnlineShop.Controllers;

public class AccountController : Controller
{
    private readonly OnlineStoreContext _context;

    public AccountController(OnlineStoreContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View(new RegisterViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var username = model.Username.Trim();

        var exists = await _context.CustomerUsers.AnyAsync(c => c.Username == username);
        if (exists)
        {
            ModelState.AddModelError(string.Empty, "Username is already taken.");
            return View(model);
        }

        var customer = new CustomerUser
        {
            Username = username,
            Password = model.Password
        };

        _context.CustomerUsers.Add(customer);
        await _context.SaveChangesAsync();

        await SignInCustomerAsync(customer);

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.IsInRole("Customer"))
        {
            return RedirectToAction("Index", "Home");
        }

        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var username = model.Username.Trim();
        var password = model.Password;

        var customer = await _context.CustomerUsers
            .FirstOrDefaultAsync(c => c.Username == username && c.Password == password);

        if (customer == null)
        {
            ModelState.AddModelError(string.Empty, "Invalid username or password.");
            return View(model);
        }

        await SignInCustomerAsync(customer);

        if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
        {
            return Redirect(model.ReturnUrl);
        }

        return RedirectToAction("Index", "Home");
    }

    [Authorize(AuthenticationSchemes = "CustomerScheme")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("CustomerScheme");
        return RedirectToAction("Index", "Home");
    }

    public IActionResult AccessDenied()
    {
        return View();
    }

    private async Task SignInCustomerAsync(CustomerUser customer)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, customer.Id.ToString()),
            new Claim(ClaimTypes.Name, customer.Username),
            new Claim(ClaimTypes.Role, "Customer")
        };

        var identity = new ClaimsIdentity(claims, "CustomerScheme");
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync("CustomerScheme", principal);
    }
}



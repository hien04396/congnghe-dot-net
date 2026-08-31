using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Data;
using OnlineShop.Models;
using OnlineShop.Services;

namespace OnlineShop.Tests.Helpers;

public sealed class FakeSession : ISession
{
    private readonly Dictionary<string, byte[]> _store = new();

    public bool IsAvailable => true;
    public string Id => "test-session";
    public IEnumerable<string> Keys => _store.Keys;

    public void Clear() => _store.Clear();
    public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    public void Remove(string key) => _store.Remove(key);
    public void Set(string key, byte[] value) => _store[key] = value;
    public bool TryGetValue(string key, out byte[] value) => _store.TryGetValue(key, out value!);
}

public static class TestFixtures
{
    public static OnlineStoreContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<OnlineStoreContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new OnlineStoreContext(options);
    }

    public static (CartService Cart, DefaultHttpContext Http, OnlineStoreContext Db) CreateCartEnvironment(
        OnlineStoreContext? existingContext = null)
    {
        var db = existingContext ?? CreateContext();
        var session = new FakeSession();
        var http = new DefaultHttpContext();
        http.Features.Set<ISessionFeature>(new SessionFeature { Session = session });
        http.Session = session;

        var accessor = new HttpContextAccessor { HttpContext = http };
        var cart = new CartService(accessor, db);
        return (cart, http, db);
    }

    public static void SetCustomerUser(HttpContext http, int userId, string username = "customer")
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(ClaimTypes.Name, username),
            new(ClaimTypes.Role, "Customer")
        };
        http.User = new ClaimsPrincipal(new ClaimsIdentity(claims, "CustomerScheme"));
    }

    public static void AttachTempData(Controller controller, HttpContext http)
    {
        controller.ControllerContext = new ControllerContext { HttpContext = http };
        controller.TempData = new TempDataDictionary(http, new NullTempDataProvider());
    }

    public static Product SeedActiveProduct(
        OnlineStoreContext db,
        string name = "Basic White Shirt",
        decimal price = 100m,
        int stock = 10,
        bool isActive = true,
        bool isFeatured = false)
    {
        var category = new ProductCategory { Name = "Shirts", Description = "Test" };
        db.ProductCategories.Add(category);
        db.SaveChanges();

        var product = new Product
        {
            Name = name,
            Price = price,
            IsActive = isActive,
            IsFeatured = isFeatured,
            CategoryId = category.Id
        };
        db.Products.Add(product);
        db.SaveChanges();

        db.ProductInventories.Add(new ProductInventory
        {
            ProductId = product.Id,
            StockQuantity = stock
        });
        db.ProductImages.Add(new ProductImage
        {
            ProductId = product.Id,
            ImageUrl = "https://example.com/shirt.png",
            IsPrimary = true
        });
        db.SaveChanges();
        return product;
    }

    public static CustomerUser SeedCustomer(OnlineStoreContext db, string username = "alice", string password = "123456")
    {
        var customer = new CustomerUser { Username = username, Password = password };
        db.CustomerUsers.Add(customer);
        db.SaveChanges();
        return customer;
    }

    private sealed class SessionFeature : ISessionFeature
    {
        public ISession Session { get; set; } = null!;
    }

    private sealed class NullTempDataProvider : ITempDataProvider
    {
        public IDictionary<string, object> LoadTempData(HttpContext context) => new Dictionary<string, object>();
        public void SaveTempData(HttpContext context, IDictionary<string, object> values) { }
    }
}

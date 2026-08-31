using Microsoft.AspNetCore.Mvc;
using OnlineShop.Controllers;
using OnlineShop.Models;
using OnlineShop.Tests.Helpers;
using OnlineShop.ViewModels;

namespace OnlineShop.Tests;

public class OrdersControllerTests
{
    [Fact]
    public async Task Index_ChiTraVeDonCuaKhachHienTai()
    {
        var db = TestFixtures.CreateContext();
        var alice = TestFixtures.SeedCustomer(db, "alice", "p");
        var bob = TestFixtures.SeedCustomer(db, "bob", "p");
        db.Orders.AddRange(
            new Order { CustomerUserId = alice.Id, TotalAmount = 10m },
            new Order { CustomerUserId = bob.Id, TotalAmount = 99m }
        );
        db.SaveChanges();

        var (_, http, _) = TestFixtures.CreateCartEnvironment(db);
        TestFixtures.SetCustomerUser(http, alice.Id, "alice");
        var controller = new OrdersController(db);
        TestFixtures.AttachTempData(controller, http);

        var result = await controller.Index();

        var view = Assert.IsType<ViewResult>(result);
        var orders = Assert.IsAssignableFrom<List<Order>>(view.Model);
        var order = Assert.Single(orders);
        Assert.Equal(alice.Id, order.CustomerUserId);
        Assert.Equal(10m, order.TotalAmount);
    }

    [Fact]
    public async Task Details_DonCuaNguoiKhac_NotFound()
    {
        var db = TestFixtures.CreateContext();
        var alice = TestFixtures.SeedCustomer(db, "alice", "p");
        var bob = TestFixtures.SeedCustomer(db, "bob", "p");
        db.Orders.Add(new Order { CustomerUserId = bob.Id, TotalAmount = 50m });
        db.SaveChanges();
        var bobOrderId = db.Orders.First().Id;

        var (_, http, _) = TestFixtures.CreateCartEnvironment(db);
        TestFixtures.SetCustomerUser(http, alice.Id, "alice");
        var controller = new OrdersController(db);
        TestFixtures.AttachTempData(controller, http);

        var result = await controller.Details(bobOrderId);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Details_DonCuaChinhMinh_TraVeView()
    {
        var db = TestFixtures.CreateContext();
        var alice = TestFixtures.SeedCustomer(db, "alice", "p");
        db.Orders.Add(new Order { CustomerUserId = alice.Id, TotalAmount = 30m });
        db.SaveChanges();
        var orderId = db.Orders.First().Id;

        var (_, http, _) = TestFixtures.CreateCartEnvironment(db);
        TestFixtures.SetCustomerUser(http, alice.Id, "alice");
        var controller = new OrdersController(db);
        TestFixtures.AttachTempData(controller, http);

        var result = await controller.Details(orderId);

        var view = Assert.IsType<ViewResult>(result);
        var order = Assert.IsType<Order>(view.Model);
        Assert.Equal(orderId, order.Id);
    }
}

public class StoreControllerTests
{
    [Fact]
    public async Task Details_SanPhamInactive_NotFound()
    {
        var db = TestFixtures.CreateContext();
        var product = TestFixtures.SeedActiveProduct(db, isActive: false);
        var controller = new StoreController(db);

        var result = await controller.Details(product.Id);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Details_IdKhongTonTai_NotFound()
    {
        var db = TestFixtures.CreateContext();
        var controller = new StoreController(db);
        Assert.IsType<NotFoundResult>(await controller.Details(999));
    }

    [Fact]
    public async Task Products_ChiHienSanPhamActive_VaLocTheoSearch()
    {
        var db = TestFixtures.CreateContext();
        TestFixtures.SeedActiveProduct(db, name: "Basic White Shirt", price: 20m, stock: 5);
        var hiddenCategory = new ProductCategory { Name = "Hidden" };
        db.ProductCategories.Add(hiddenCategory);
        db.SaveChanges();
        db.Products.Add(new Product
        {
            Name = "Hidden Shirt",
            Price = 1m,
            IsActive = false,
            CategoryId = hiddenCategory.Id
        });
        db.SaveChanges();

        var controller = new StoreController(db);
        var result = await controller.Products(categoryId: null, search: "Shirt", page: 1, pageSize: 8);

        var view = Assert.IsType<ViewResult>(result);
        var vm = Assert.IsType<ProductListViewModel>(view.Model);
        Assert.All(vm.Products, p => Assert.True(p.IsActive));
        Assert.Contains(vm.Products, p => p.Name.Contains("Shirt"));
        Assert.DoesNotContain(vm.Products, p => p.Name == "Hidden Shirt");
    }

    [Fact]
    public async Task Products_SearchKhoangTrang_KhongLocTen()
    {
        var db = TestFixtures.CreateContext();
        TestFixtures.SeedActiveProduct(db, name: "Blue Jeans");
        var controller = new StoreController(db);

        var result = await controller.Products(null, search: "   ", page: 1, pageSize: 8);
        var vm = Assert.IsType<ProductListViewModel>(Assert.IsType<ViewResult>(result).Model);

        Assert.Contains(vm.Products, p => p.Name == "Blue Jeans");
    }
}

using Microsoft.AspNetCore.Mvc;
using OnlineShop.Controllers;
using OnlineShop.Models;
using OnlineShop.Tests.Helpers;

namespace OnlineShop.Tests;

/// <summary>
/// Integration test CartController.Checkout với CartService + DbContext InMemory.
/// Source: OnlineShop/Controllers/CartController.cs
/// </summary>
public class CartControllerCheckoutTests
{
    [Fact]
    public async Task Checkout_GioTrong_BaoLoiVaXoaGio()
    {
        // Arrange
        var (cart, http, db) = TestFixtures.CreateCartEnvironment();
        var customer = TestFixtures.SeedCustomer(db);
        TestFixtures.SetCustomerUser(http, customer.Id);
        var controller = new CartController(cart, db);
        TestFixtures.AttachTempData(controller, http);

        // Act
        var result = await controller.Checkout();

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        Assert.Equal("Nothing to checkout. All items are out of stock.", controller.TempData["Error"]);
        Assert.Empty(cart.GetCart());
    }

    [Fact]
    public async Task Checkout_ChiConHangHetHang_BaoLoi()
    {
        // Arrange
        var db = TestFixtures.CreateContext();
        var product = TestFixtures.SeedActiveProduct(db, stock: 0);
        var customer = TestFixtures.SeedCustomer(db);
        var (cart, http, _) = TestFixtures.CreateCartEnvironment(db);
        TestFixtures.SetCustomerUser(http, customer.Id);
        await cart.AddToCartAsync(product.Id, 1);

        var controller = new CartController(cart, db);
        TestFixtures.AttachTempData(controller, http);

        // Act
        var result = await controller.Checkout();

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        Assert.Equal("Nothing to checkout. All items are out of stock.", controller.TempData["Error"]);
        Assert.Empty(db.Orders);
    }

    [Fact]
    public async Task Checkout_TonKhoGiamSauKhiThemGio_ClampRoiVanDatHang()
    {
        // Arrange: Checkout luôn gọi RefreshStockFlagsAsync trước nên quantity bị kẹp theo stock hiện tại.
        // Nhánh TempData "Some items no longer have enough stock" chỉ xảy ra nếu tồn kho
        // thay đổi giữa hai lần đọc DB trong cùng request (đua tranh) — không tái hiện tuần tự.
        var db = TestFixtures.CreateContext();
        var product = TestFixtures.SeedActiveProduct(db, price: 10m, stock: 5);
        var customer = TestFixtures.SeedCustomer(db);
        var (cart, http, _) = TestFixtures.CreateCartEnvironment(db);
        TestFixtures.SetCustomerUser(http, customer.Id);
        await cart.AddToCartAsync(product.Id, 5);

        var inventory = db.ProductInventories.First(i => i.ProductId == product.Id);
        inventory.StockQuantity = 2;
        db.SaveChanges();

        var controller = new CartController(cart, db);
        TestFixtures.AttachTempData(controller, http);

        // Act
        var result = await controller.Checkout();

        // Assert — đặt hàng với số lượng đã kẹp = 2, tồn kho còn 0
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Details", redirect.ActionName);
        var order = Assert.Single(db.Orders);
        Assert.Equal(20m, order.TotalAmount);
        Assert.Equal(2, Assert.Single(order.Items).Quantity);
        Assert.Equal(0, db.ProductInventories.First().StockQuantity);
    }

    [Fact]
    public async Task Checkout_HopLe_TaoDonGiamKhoVaXoaGio()
    {
        // Arrange
        var db = TestFixtures.CreateContext();
        var product = TestFixtures.SeedActiveProduct(db, price: 20m, stock: 10);
        var customer = TestFixtures.SeedCustomer(db);
        var (cart, http, _) = TestFixtures.CreateCartEnvironment(db);
        TestFixtures.SetCustomerUser(http, customer.Id);
        await cart.AddToCartAsync(product.Id, 3);

        var controller = new CartController(cart, db);
        TestFixtures.AttachTempData(controller, http);

        // Act
        var result = await controller.Checkout();

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Details", redirect.ActionName);
        Assert.Equal("Orders", redirect.ControllerName);
        Assert.Equal("Order placed successfully!", controller.TempData["Success"]);
        Assert.Empty(cart.GetCart());

        var order = Assert.Single(db.Orders);
        Assert.Equal(customer.Id, order.CustomerUserId);
        Assert.Equal(60m, order.TotalAmount);
        var line = Assert.Single(order.Items);
        Assert.Equal(product.Id, line.ProductId);
        Assert.Equal(3, line.Quantity);
        Assert.Equal(20m, line.UnitPrice);
        Assert.Equal(60m, line.LineTotal);
        Assert.Equal(7, db.ProductInventories.First().StockQuantity);
    }

    [Fact]
    public async Task Checkout_CoPromo_LuuGiaDaGiamVaoOrderItem()
    {
        var db = TestFixtures.CreateContext();
        var product = TestFixtures.SeedActiveProduct(db, price: 100m, stock: 5);
        db.ProductPromos.Add(new ProductPromo
        {
            ProductId = product.Id,
            AmountOff = 40m,
            IsActive = true
        });
        db.SaveChanges();
        var customer = TestFixtures.SeedCustomer(db);
        var (cart, http, _) = TestFixtures.CreateCartEnvironment(db);
        TestFixtures.SetCustomerUser(http, customer.Id);
        await cart.AddToCartAsync(product.Id, 2);

        var controller = new CartController(cart, db);
        TestFixtures.AttachTempData(controller, http);

        var result = await controller.Checkout();

        Assert.IsType<RedirectToActionResult>(result);
        var order = Assert.Single(db.Orders);
        Assert.Equal(120m, order.TotalAmount);
        Assert.Equal(60m, Assert.Single(order.Items).UnitPrice);
    }
}

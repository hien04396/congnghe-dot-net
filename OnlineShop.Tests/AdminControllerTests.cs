using Microsoft.AspNetCore.Mvc;
using OnlineShop.Controllers;
using OnlineShop.Models;
using OnlineShop.Tests.Helpers;

namespace OnlineShop.Tests;

public class AdminUsersControllerTests
{
    [Fact]
    public async Task Create_GanIsDefaultFalse()
    {
        var db = TestFixtures.CreateContext();
        var controller = new AdminUsersController(db);
        TestFixtures.AttachTempData(controller, new Microsoft.AspNetCore.Http.DefaultHttpContext());

        var result = await controller.Create(new AdminUser
        {
            Username = "staff",
            Password = "secret",
            IsDefault = true
        });

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        var saved = Assert.Single(db.AdminUsers);
        Assert.False(saved.IsDefault);
        Assert.Equal("staff", saved.Username);
    }

    [Fact]
    public async Task Edit_Get_AdminMacDinh_RedirectVaBaoLoi()
    {
        var db = TestFixtures.CreateContext();
        db.AdminUsers.Add(new AdminUser { Username = "admin", Password = "admin", IsDefault = true });
        db.SaveChanges();
        var id = db.AdminUsers.First().Id;

        var controller = new AdminUsersController(db);
        TestFixtures.AttachTempData(controller, new Microsoft.AspNetCore.Http.DefaultHttpContext());

        var result = await controller.Edit(id);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        Assert.Equal("Default admin cannot be edited here.", controller.TempData["Error"]);
    }

    [Fact]
    public async Task Edit_Get_IdKhongTonTai_NotFound()
    {
        var db = TestFixtures.CreateContext();
        var controller = new AdminUsersController(db);
        TestFixtures.AttachTempData(controller, new Microsoft.AspNetCore.Http.DefaultHttpContext());

        var result = await controller.Edit(999);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteConfirmed_AdminMacDinh_KhongXoa()
    {
        var db = TestFixtures.CreateContext();
        db.AdminUsers.Add(new AdminUser { Username = "admin", Password = "admin", IsDefault = true });
        db.SaveChanges();
        var id = db.AdminUsers.First().Id;

        var controller = new AdminUsersController(db);
        TestFixtures.AttachTempData(controller, new Microsoft.AspNetCore.Http.DefaultHttpContext());

        var result = await controller.DeleteConfirmed(id);

        Assert.IsType<RedirectToActionResult>(result);
        Assert.Single(db.AdminUsers);
    }

    [Fact]
    public async Task DeleteConfirmed_AdminThuong_XoaThanhCong()
    {
        var db = TestFixtures.CreateContext();
        db.AdminUsers.Add(new AdminUser { Username = "staff", Password = "x", IsDefault = false });
        db.SaveChanges();
        var id = db.AdminUsers.First().Id;

        var controller = new AdminUsersController(db);
        TestFixtures.AttachTempData(controller, new Microsoft.AspNetCore.Http.DefaultHttpContext());

        await controller.DeleteConfirmed(id);

        Assert.Empty(db.AdminUsers);
    }

    [Fact]
    public async Task Edit_Post_IdKhongKhop_NotFound()
    {
        var db = TestFixtures.CreateContext();
        var controller = new AdminUsersController(db);
        TestFixtures.AttachTempData(controller, new Microsoft.AspNetCore.Http.DefaultHttpContext());

        var result = await controller.Edit(1, new AdminUser { Id = 2, Username = "a", Password = "b" });

        Assert.IsType<NotFoundResult>(result);
    }
}

public class AdminProductInventoryControllerTests
{
    [Fact]
    public async Task ManageStock_Get_ChuaCoInventory_TaoBanGhiStock0()
    {
        var db = TestFixtures.CreateContext();
        var product = TestFixtures.SeedActiveProduct(db, stock: 5);
        db.ProductInventories.RemoveRange(db.ProductInventories);
        db.SaveChanges();

        var controller = new AdminProductInventoryController(db);
        TestFixtures.AttachTempData(controller, new Microsoft.AspNetCore.Http.DefaultHttpContext());

        var result = await controller.ManageStock(product.Id);

        var view = Assert.IsType<ViewResult>(result);
        var inventory = Assert.IsType<ProductInventory>(view.Model);
        Assert.Equal(0, inventory.StockQuantity);
        Assert.Single(db.ProductInventories);
    }

    [Fact]
    public async Task ManageStock_Post_GanSoLuongTuyetDoi()
    {
        var db = TestFixtures.CreateContext();
        var product = TestFixtures.SeedActiveProduct(db, stock: 10);
        var inventory = db.ProductInventories.First(i => i.ProductId == product.Id);

        var controller = new AdminProductInventoryController(db);
        TestFixtures.AttachTempData(controller, new Microsoft.AspNetCore.Http.DefaultHttpContext());

        var result = await controller.ManageStock(inventory.Id, stockQuantity: 25, adjustment: 0);

        Assert.IsType<ViewResult>(result);
        Assert.Equal(25, db.ProductInventories.First().StockQuantity);
        Assert.Equal("Stock updated successfully! New quantity: 25", controller.TempData["Success"]);
    }

    [Fact]
    public async Task ManageStock_Post_DieuChinhAmVuotTonKho_TuChoi()
    {
        var db = TestFixtures.CreateContext();
        var product = TestFixtures.SeedActiveProduct(db, stock: 3);
        var inventory = db.ProductInventories.First(i => i.ProductId == product.Id);

        var controller = new AdminProductInventoryController(db);
        TestFixtures.AttachTempData(controller, new Microsoft.AspNetCore.Http.DefaultHttpContext());

        var result = await controller.ManageStock(inventory.Id, stockQuantity: 0, adjustment: -5);

        Assert.IsType<ViewResult>(result);
        Assert.Equal("Stock quantity cannot be negative.", controller.TempData["Error"]);
        Assert.Equal(3, db.ProductInventories.First().StockQuantity);
    }

    [Fact]
    public async Task ManageStock_Post_DieuChinhCong_TangTonKho()
    {
        var db = TestFixtures.CreateContext();
        var product = TestFixtures.SeedActiveProduct(db, stock: 3);
        var inventory = db.ProductInventories.First(i => i.ProductId == product.Id);

        var controller = new AdminProductInventoryController(db);
        TestFixtures.AttachTempData(controller, new Microsoft.AspNetCore.Http.DefaultHttpContext());

        await controller.ManageStock(inventory.Id, stockQuantity: 0, adjustment: 2);

        Assert.Equal(5, db.ProductInventories.First().StockQuantity);
    }

    [Fact]
    public async Task ManageStock_Post_IdKhongTonTai_NotFound()
    {
        var db = TestFixtures.CreateContext();
        var controller = new AdminProductInventoryController(db);
        TestFixtures.AttachTempData(controller, new Microsoft.AspNetCore.Http.DefaultHttpContext());

        var result = await controller.ManageStock(id: 999, stockQuantity: 1);
        Assert.IsType<NotFoundResult>(result);
    }
}

public class AdminProductImagesControllerTests
{
    [Fact]
    public async Task Add_UrlRong_BaoLoi()
    {
        var db = TestFixtures.CreateContext();
        var product = TestFixtures.SeedActiveProduct(db);
        var controller = new AdminProductImagesController(db);
        TestFixtures.AttachTempData(controller, new Microsoft.AspNetCore.Http.DefaultHttpContext());

        var result = await controller.Add(product.Id, "  ", isPrimary: false);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        Assert.Equal("Image URL is required.", controller.TempData["Error"]);
    }

    [Fact]
    public async Task Add_UrlTuongDoi_TuChoi()
    {
        var db = TestFixtures.CreateContext();
        var product = TestFixtures.SeedActiveProduct(db);
        var controller = new AdminProductImagesController(db);
        TestFixtures.AttachTempData(controller, new Microsoft.AspNetCore.Http.DefaultHttpContext());

        await controller.Add(product.Id, "/images/sample-placeholder.png");

        Assert.Equal(
            "Please enter a valid URL (must start with http:// or https://).",
            controller.TempData["Error"]);
    }

    [Fact]
    public async Task Add_UrlHttpsVaIsPrimary_BoPrimaryCu()
    {
        var db = TestFixtures.CreateContext();
        var product = TestFixtures.SeedActiveProduct(db);
        var controller = new AdminProductImagesController(db);
        TestFixtures.AttachTempData(controller, new Microsoft.AspNetCore.Http.DefaultHttpContext());

        var result = await controller.Add(product.Id, "https://cdn.example.com/new.png", isPrimary: true);

        Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Image added successfully.", controller.TempData["Success"]);
        Assert.Equal(1, db.ProductImages.Count(i => i.ProductId == product.Id && i.IsPrimary));
        Assert.Contains(db.ProductImages, i => i.ImageUrl == "https://cdn.example.com/new.png" && i.IsPrimary);
    }

    [Fact]
    public async Task MakePrimary_ChuyenAnhChinh()
    {
        var db = TestFixtures.CreateContext();
        var product = TestFixtures.SeedActiveProduct(db);
        var second = new ProductImage
        {
            ProductId = product.Id,
            ImageUrl = "https://cdn.example.com/b.png",
            IsPrimary = false
        };
        db.ProductImages.Add(second);
        db.SaveChanges();

        var controller = new AdminProductImagesController(db);
        TestFixtures.AttachTempData(controller, new Microsoft.AspNetCore.Http.DefaultHttpContext());

        await controller.MakePrimary(second.Id);

        Assert.True(db.ProductImages.First(i => i.Id == second.Id).IsPrimary);
        Assert.Equal(1, db.ProductImages.Count(i => i.IsPrimary));
    }
}

public class AdminProductPromosControllerTests
{
    [Fact]
    public async Task Create_PromoActiveMoi_VoHieuHoaPromoActiveCu()
    {
        var db = TestFixtures.CreateContext();
        var product = TestFixtures.SeedActiveProduct(db);
        db.ProductPromos.Add(new ProductPromo
        {
            ProductId = product.Id,
            AmountOff = 5m,
            IsActive = true
        });
        db.SaveChanges();
        var oldId = db.ProductPromos.First().Id;

        var controller = new AdminProductPromosController(db);
        TestFixtures.AttachTempData(controller, new Microsoft.AspNetCore.Http.DefaultHttpContext());

        var result = await controller.Create(new ProductPromo
        {
            ProductId = product.Id,
            AmountOff = 12m,
            IsActive = true
        }, returnProductId: product.Id);

        Assert.IsType<RedirectToActionResult>(result);
        Assert.False(db.ProductPromos.First(p => p.Id == oldId).IsActive);
        Assert.Equal(1, db.ProductPromos.Count(p => p.ProductId == product.Id && p.IsActive));
    }

    [Fact]
    public async Task Create_PromoKhongActive_KhongVoHieuHoaPromoKhac()
    {
        var db = TestFixtures.CreateContext();
        var product = TestFixtures.SeedActiveProduct(db);
        db.ProductPromos.Add(new ProductPromo
        {
            ProductId = product.Id,
            AmountOff = 5m,
            IsActive = true
        });
        db.SaveChanges();

        var controller = new AdminProductPromosController(db);
        TestFixtures.AttachTempData(controller, new Microsoft.AspNetCore.Http.DefaultHttpContext());

        await controller.Create(new ProductPromo
        {
            ProductId = product.Id,
            AmountOff = 8m,
            IsActive = false
        }, null);

        Assert.Equal(1, db.ProductPromos.Count(p => p.IsActive));
        Assert.Equal(2, db.ProductPromos.Count());
    }
}

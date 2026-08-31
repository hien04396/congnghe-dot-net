using Microsoft.EntityFrameworkCore;
using OnlineShop.Models;
using OnlineShop.Services;
using OnlineShop.Tests.Helpers;

namespace OnlineShop.Tests;

public class CartServiceTests
{
    [Fact]
    public async Task AddToCartAsync_SanPhamKhongTonTai_KhongThemVaoGio()
    {
        var (cart, _, _) = TestFixtures.CreateCartEnvironment();

        await cart.AddToCartAsync(productId: 999, quantity: 1);

        Assert.Empty(cart.GetCart());
    }

    [Fact]
    public async Task AddToCartAsync_SanPhamKhongActive_KhongThemVaoGio()
    {
        var db = TestFixtures.CreateContext();
        var product = TestFixtures.SeedActiveProduct(db, isActive: false);
        var (cart, _, _) = TestFixtures.CreateCartEnvironment(db);

        await cart.AddToCartAsync(product.Id, 1);

        Assert.Empty(cart.GetCart());
    }

    [Fact]
    public async Task AddToCartAsync_SanPhamMoiConHang_ThemDungSoLuongVaGia()
    {
        var db = TestFixtures.CreateContext();
        var product = TestFixtures.SeedActiveProduct(db, price: 19.99m, stock: 10);
        var (cart, _, _) = TestFixtures.CreateCartEnvironment(db);

        await cart.AddToCartAsync(product.Id, 2);

        var item = Assert.Single(cart.GetCart());
        Assert.Equal(product.Id, item.ProductId);
        Assert.Equal(2, item.Quantity);
        Assert.Equal(19.99m, item.UnitPrice);
        Assert.Equal(19.99m, item.OriginalPrice);
        Assert.Null(item.PromoDiscount);
        Assert.False(item.IsOutOfStock);
        Assert.Equal("https://example.com/shirt.png", item.ImageUrl);
    }

    [Fact]
    public async Task AddToCartAsync_SoLuongVuotTonKho_GioiHanBangStock()
    {
        var db = TestFixtures.CreateContext();
        var product = TestFixtures.SeedActiveProduct(db, stock: 3);
        var (cart, _, _) = TestFixtures.CreateCartEnvironment(db);

        await cart.AddToCartAsync(product.Id, 10);

        var item = Assert.Single(cart.GetCart());
        Assert.Equal(3, item.Quantity);
        Assert.False(item.IsOutOfStock);
    }

    [Fact]
    public async Task AddToCartAsync_TonKhoBang0_DanhDauHetHangVaQuantity0()
    {
        var db = TestFixtures.CreateContext();
        var product = TestFixtures.SeedActiveProduct(db, stock: 0);
        var (cart, _, _) = TestFixtures.CreateCartEnvironment(db);

        await cart.AddToCartAsync(product.Id, 1);

        var item = Assert.Single(cart.GetCart());
        Assert.True(item.IsOutOfStock);
        Assert.Equal(0, item.Quantity);
    }

    [Fact]
    public async Task AddToCartAsync_SanPhamDaCoTrongGio_CongDonSoLuong()
    {
        var db = TestFixtures.CreateContext();
        var product = TestFixtures.SeedActiveProduct(db, stock: 10);
        var (cart, _, _) = TestFixtures.CreateCartEnvironment(db);
        await cart.AddToCartAsync(product.Id, 2);

        await cart.AddToCartAsync(product.Id, 3);

        var item = Assert.Single(cart.GetCart());
        Assert.Equal(5, item.Quantity);
    }

    [Fact]
    public async Task AddToCartAsync_CoPromoActive_ApDungGiaGiam()
    {
        var db = TestFixtures.CreateContext();
        var product = TestFixtures.SeedActiveProduct(db, price: 100m, stock: 5);
        db.ProductPromos.Add(new ProductPromo
        {
            ProductId = product.Id,
            AmountOff = 20m,
            IsActive = true,
            StartDate = DateTime.UtcNow.AddDays(-1)
        });
        db.SaveChanges();
        var (cart, _, _) = TestFixtures.CreateCartEnvironment(db);

        await cart.AddToCartAsync(product.Id, 1);

        var item = Assert.Single(cart.GetCart());
        Assert.Equal(100m, item.OriginalPrice);
        Assert.Equal(80m, item.UnitPrice);
        Assert.Equal(20m, item.PromoDiscount);
        Assert.True(item.HasPromo);
    }

    [Fact]
    public async Task AddToCartAsync_PromoAmountOffLonHonGia_UnitPriceBang0()
    {
        var db = TestFixtures.CreateContext();
        var product = TestFixtures.SeedActiveProduct(db, price: 10m, stock: 5);
        db.ProductPromos.Add(new ProductPromo
        {
            ProductId = product.Id,
            AmountOff = 50m,
            IsActive = true
        });
        db.SaveChanges();
        var (cart, _, _) = TestFixtures.CreateCartEnvironment(db);

        await cart.AddToCartAsync(product.Id, 1);

        var item = Assert.Single(cart.GetCart());
        Assert.Equal(0m, item.UnitPrice);
        Assert.Equal(10m, item.PromoDiscount);
    }

    [Fact]
    public async Task AddToCartAsync_PromoKhongActive_KhongGiamGia()
    {
        var db = TestFixtures.CreateContext();
        var product = TestFixtures.SeedActiveProduct(db, price: 100m, stock: 5);
        db.ProductPromos.Add(new ProductPromo
        {
            ProductId = product.Id,
            AmountOff = 20m,
            IsActive = false
        });
        db.SaveChanges();
        var (cart, _, _) = TestFixtures.CreateCartEnvironment(db);

        await cart.AddToCartAsync(product.Id, 1);

        var item = Assert.Single(cart.GetCart());
        Assert.Equal(100m, item.UnitPrice);
        Assert.Null(item.PromoDiscount);
    }

    [Fact]
    public async Task AddToCartAsync_NhieuPromoActive_ChonPromoStartDateMoiNhat()
    {
        var db = TestFixtures.CreateContext();
        var product = TestFixtures.SeedActiveProduct(db, price: 100m, stock: 5);
        db.ProductPromos.AddRange(
            new ProductPromo { ProductId = product.Id, AmountOff = 10m, IsActive = true, StartDate = DateTime.UtcNow.AddDays(-10) },
            new ProductPromo { ProductId = product.Id, AmountOff = 30m, IsActive = true, StartDate = DateTime.UtcNow }
        );
        db.SaveChanges();
        var (cart, _, _) = TestFixtures.CreateCartEnvironment(db);

        await cart.AddToCartAsync(product.Id, 1);

        var item = Assert.Single(cart.GetCart());
        Assert.Equal(70m, item.UnitPrice);
        Assert.Equal(30m, item.PromoDiscount);
    }

    [Fact]
    public async Task UpdateQuantity_ProductKhongCoTrongGio_KhongThayDoi()
    {
        var db = TestFixtures.CreateContext();
        var product = TestFixtures.SeedActiveProduct(db);
        var (cart, _, _) = TestFixtures.CreateCartEnvironment(db);
        await cart.AddToCartAsync(product.Id, 1);

        cart.UpdateQuantity(productId: 999, quantity: 5);

        Assert.Equal(1, Assert.Single(cart.GetCart()).Quantity);
    }

    [Fact]
    public async Task UpdateQuantity_SoLuongBang0_XoaKhoiGio()
    {
        var db = TestFixtures.CreateContext();
        var product = TestFixtures.SeedActiveProduct(db);
        var (cart, _, _) = TestFixtures.CreateCartEnvironment(db);
        await cart.AddToCartAsync(product.Id, 2);

        cart.UpdateQuantity(product.Id, 0);

        Assert.Empty(cart.GetCart());
    }

    [Fact]
    public async Task UpdateQuantity_SoLuongAm_XoaKhoiGio()
    {
        var db = TestFixtures.CreateContext();
        var product = TestFixtures.SeedActiveProduct(db);
        var (cart, _, _) = TestFixtures.CreateCartEnvironment(db);
        await cart.AddToCartAsync(product.Id, 2);

        cart.UpdateQuantity(product.Id, -1);

        Assert.Empty(cart.GetCart());
    }

    [Fact]
    public async Task UpdateQuantity_SanPhamHetHang_KhongDoiQuantity()
    {
        var db = TestFixtures.CreateContext();
        var product = TestFixtures.SeedActiveProduct(db, stock: 0);
        var (cart, _, _) = TestFixtures.CreateCartEnvironment(db);
        await cart.AddToCartAsync(product.Id, 1);
        Assert.True(Assert.Single(cart.GetCart()).IsOutOfStock);

        cart.UpdateQuantity(product.Id, 5);

        Assert.Equal(0, Assert.Single(cart.GetCart()).Quantity);
    }

    [Fact]
    public async Task UpdateQuantity_HopLe_CapNhatSoLuong()
    {
        var db = TestFixtures.CreateContext();
        var product = TestFixtures.SeedActiveProduct(db, stock: 10);
        var (cart, _, _) = TestFixtures.CreateCartEnvironment(db);
        await cart.AddToCartAsync(product.Id, 1);

        cart.UpdateQuantity(product.Id, 4);

        Assert.Equal(4, Assert.Single(cart.GetCart()).Quantity);
    }

    [Fact]
    public async Task RemoveItem_XoaDungSanPham()
    {
        var db = TestFixtures.CreateContext();
        var product = TestFixtures.SeedActiveProduct(db);
        var (cart, _, _) = TestFixtures.CreateCartEnvironment(db);
        await cart.AddToCartAsync(product.Id, 1);

        cart.RemoveItem(product.Id);

        Assert.Empty(cart.GetCart());
    }

    [Fact]
    public async Task Clear_XoaToanBoGio()
    {
        var db = TestFixtures.CreateContext();
        var product = TestFixtures.SeedActiveProduct(db);
        var (cart, _, _) = TestFixtures.CreateCartEnvironment(db);
        await cart.AddToCartAsync(product.Id, 1);

        cart.Clear();

        Assert.Empty(cart.GetCart());
    }

    [Fact]
    public async Task GetTotal_KhongTinhSanPhamHetHang()
    {
        var db = TestFixtures.CreateContext();
        var inStock = TestFixtures.SeedActiveProduct(db, name: "InStock", price: 10m, stock: 5);

        var outCategory = new ProductCategory { Name = "Pants" };
        db.ProductCategories.Add(outCategory);
        db.SaveChanges();
        var outStock = new Product
        {
            Name = "OutStock",
            Price = 50m,
            IsActive = true,
            CategoryId = outCategory.Id
        };
        db.Products.Add(outStock);
        db.SaveChanges();
        db.ProductInventories.Add(new ProductInventory { ProductId = outStock.Id, StockQuantity = 0 });
        db.SaveChanges();

        var (cart, _, _) = TestFixtures.CreateCartEnvironment(db);
        await cart.AddToCartAsync(inStock.Id, 2);
        await cart.AddToCartAsync(outStock.Id, 1);

        var total = cart.GetTotal();

        Assert.Equal(20m, total);
    }

    [Fact]
    public async Task RefreshStockFlagsAsync_TonKhoGiamDuoiQuantity_ClampVaGiuHang()
    {
        var db = TestFixtures.CreateContext();
        var product = TestFixtures.SeedActiveProduct(db, stock: 10);
        var (cart, _, _) = TestFixtures.CreateCartEnvironment(db);
        await cart.AddToCartAsync(product.Id, 5);

        var inventory = await db.ProductInventories.FirstAsync(i => i.ProductId == product.Id);
        inventory.StockQuantity = 2;
        await db.SaveChangesAsync();

        await cart.RefreshStockFlagsAsync();

        var item = Assert.Single(cart.GetCart());
        Assert.Equal(2, item.Quantity);
        Assert.False(item.IsOutOfStock);
    }

    [Fact]
    public async Task RefreshStockFlagsAsync_TonKhoVe0_DanhDauHetHang()
    {
        var db = TestFixtures.CreateContext();
        var product = TestFixtures.SeedActiveProduct(db, stock: 5);
        var (cart, _, _) = TestFixtures.CreateCartEnvironment(db);
        await cart.AddToCartAsync(product.Id, 1);

        var inventory = await db.ProductInventories.FirstAsync(i => i.ProductId == product.Id);
        inventory.StockQuantity = 0;
        await db.SaveChangesAsync();

        await cart.RefreshStockFlagsAsync();

        var item = Assert.Single(cart.GetCart());
        Assert.True(item.IsOutOfStock);
        Assert.Equal(0, item.Quantity);
    }

    [Fact]
    public async Task RefreshPricesAsync_PromoMoi_CapNhatGiaTrongGio()
    {
        var db = TestFixtures.CreateContext();
        var product = TestFixtures.SeedActiveProduct(db, price: 100m, stock: 5);
        var (cart, _, _) = TestFixtures.CreateCartEnvironment(db);
        await cart.AddToCartAsync(product.Id, 1);
        Assert.Equal(100m, Assert.Single(cart.GetCart()).UnitPrice);

        db.ProductPromos.Add(new ProductPromo
        {
            ProductId = product.Id,
            AmountOff = 15m,
            IsActive = true,
            StartDate = DateTime.UtcNow
        });
        db.SaveChanges();

        await cart.RefreshPricesAsync();

        var item = Assert.Single(cart.GetCart());
        Assert.Equal(85m, item.UnitPrice);
        Assert.Equal(15m, item.PromoDiscount);
    }
}

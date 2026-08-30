using Microsoft.EntityFrameworkCore;
using OnlineShop.Models;
using OnlineShop.Services;
using OnlineShop.Tests.Helpers;

namespace OnlineShop.Tests;

/// <summary>
/// Unit test CartService — logic giỏ hàng, tồn kho và khuyến mãi.
/// Kỹ thuật: Equivalence Partitioning, BVA, White-box (branch/path), Module Testing.
/// Source: OnlineShop/Services/CartService.cs
/// </summary>
public class CartServiceTests
{
    // ---------- AddToCartAsync ----------

    [Fact]
    public async Task AddToCartAsync_SanPhamKhongTonTai_KhongThemVaoGio()
    {
        // Arrange
        var (cart, _, _) = TestFixtures.CreateCartEnvironment();

        // Act
        await cart.AddToCartAsync(productId: 999, quantity: 1);

        // Assert
        Assert.Empty(cart.GetCart());
    }

    [Fact]
    public async Task AddToCartAsync_SanPhamKhongActive_KhongThemVaoGio()
    {
        // Arrange
        var db = TestFixtures.CreateContext();
        var product = TestFixtures.SeedActiveProduct(db, isActive: false);
        var (cart, _, _) = TestFixtures.CreateCartEnvironment(db);

        // Act
        await cart.AddToCartAsync(product.Id, 1);

        // Assert
        Assert.Empty(cart.GetCart());
    }

    [Fact]
    public async Task AddToCartAsync_SanPhamMoiConHang_ThemDungSoLuongVaGia()
    {
        // Arrange
        var db = TestFixtures.CreateContext();
        var product = TestFixtures.SeedActiveProduct(db, price: 19.99m, stock: 10);
        var (cart, _, _) = TestFixtures.CreateCartEnvironment(db);

        // Act
        await cart.AddToCartAsync(product.Id, 2);

        // Assert
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
        // Arrange — biên: desiredQty > stock
        var db = TestFixtures.CreateContext();
        var product = TestFixtures.SeedActiveProduct(db, stock: 3);
        var (cart, _, _) = TestFixtures.CreateCartEnvironment(db);

        // Act
        await cart.AddToCartAsync(product.Id, 10);

        // Assert
        var item = Assert.Single(cart.GetCart());
        Assert.Equal(3, item.Quantity);
        Assert.False(item.IsOutOfStock);
    }

    [Fact]
    public async Task AddToCartAsync_TonKhoBang0_DanhDauHetHangVaQuantity0()
    {
        // Arrange — biên stock = 0
        var db = TestFixtures.CreateContext();
        var product = TestFixtures.SeedActiveProduct(db, stock: 0);
        var (cart, _, _) = TestFixtures.CreateCartEnvironment(db);

        // Act
        await cart.AddToCartAsync(product.Id, 1);

        // Assert
        var item = Assert.Single(cart.GetCart());
        Assert.True(item.IsOutOfStock);
        Assert.Equal(0, item.Quantity);
    }

    [Fact]
    public async Task AddToCartAsync_SanPhamDaCoTrongGio_CongDonSoLuong()
    {
        // Arrange
        var db = TestFixtures.CreateContext();
        var product = TestFixtures.SeedActiveProduct(db, stock: 10);
        var (cart, _, _) = TestFixtures.CreateCartEnvironment(db);
        await cart.AddToCartAsync(product.Id, 2);

        // Act
        await cart.AddToCartAsync(product.Id, 3);

        // Assert
        var item = Assert.Single(cart.GetCart());
        Assert.Equal(5, item.Quantity);
    }

    [Fact]
    public async Task AddToCartAsync_CoPromoActive_ApDungGiaGiam()
    {
        // Arrange
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

        // Act
        await cart.AddToCartAsync(product.Id, 1);

        // Assert
        var item = Assert.Single(cart.GetCart());
        Assert.Equal(100m, item.OriginalPrice);
        Assert.Equal(80m, item.UnitPrice);
        Assert.Equal(20m, item.PromoDiscount);
        Assert.True(item.HasPromo);
    }

    [Fact]
    public async Task AddToCartAsync_PromoAmountOffLonHonGia_UnitPriceBang0()
    {
        // Arrange — biên AmountOff > Price
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

        // Act
        await cart.AddToCartAsync(product.Id, 1);

        // Assert
        var item = Assert.Single(cart.GetCart());
        Assert.Equal(0m, item.UnitPrice);
        Assert.Equal(10m, item.PromoDiscount); // Math.Min(AmountOff, Price)
    }

    [Fact]
    public async Task AddToCartAsync_PromoKhongActive_KhongGiamGia()
    {
        // Arrange
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

        // Act
        await cart.AddToCartAsync(product.Id, 1);

        // Assert
        var item = Assert.Single(cart.GetCart());
        Assert.Equal(100m, item.UnitPrice);
        Assert.Null(item.PromoDiscount);
    }

    [Fact]
    public async Task AddToCartAsync_NhieuPromoActive_ChonPromoStartDateMoiNhat()
    {
        // Arrange — logic OrderByDescending(StartDate).FirstOrDefault
        var db = TestFixtures.CreateContext();
        var product = TestFixtures.SeedActiveProduct(db, price: 100m, stock: 5);
        db.ProductPromos.AddRange(
            new ProductPromo { ProductId = product.Id, AmountOff = 10m, IsActive = true, StartDate = DateTime.UtcNow.AddDays(-10) },
            new ProductPromo { ProductId = product.Id, AmountOff = 30m, IsActive = true, StartDate = DateTime.UtcNow }
        );
        db.SaveChanges();
        var (cart, _, _) = TestFixtures.CreateCartEnvironment(db);

        // Act
        await cart.AddToCartAsync(product.Id, 1);

        // Assert
        var item = Assert.Single(cart.GetCart());
        Assert.Equal(70m, item.UnitPrice);
        Assert.Equal(30m, item.PromoDiscount);
    }

    // ---------- UpdateQuantity ----------

    [Fact]
    public async Task UpdateQuantity_ProductKhongCoTrongGio_KhongThayDoi()
    {
        // Arrange
        var db = TestFixtures.CreateContext();
        var product = TestFixtures.SeedActiveProduct(db);
        var (cart, _, _) = TestFixtures.CreateCartEnvironment(db);
        await cart.AddToCartAsync(product.Id, 1);

        // Act
        cart.UpdateQuantity(productId: 999, quantity: 5);

        // Assert
        Assert.Equal(1, Assert.Single(cart.GetCart()).Quantity);
    }

    [Fact]
    public async Task UpdateQuantity_SoLuongBang0_XoaKhoiGio()
    {
        // Arrange — biên quantity = 0
        var db = TestFixtures.CreateContext();
        var product = TestFixtures.SeedActiveProduct(db);
        var (cart, _, _) = TestFixtures.CreateCartEnvironment(db);
        await cart.AddToCartAsync(product.Id, 2);

        // Act
        cart.UpdateQuantity(product.Id, 0);

        // Assert
        Assert.Empty(cart.GetCart());
    }

    [Fact]
    public async Task UpdateQuantity_SoLuongAm_XoaKhoiGio()
    {
        // Arrange — phân vùng quantity < 0
        var db = TestFixtures.CreateContext();
        var product = TestFixtures.SeedActiveProduct(db);
        var (cart, _, _) = TestFixtures.CreateCartEnvironment(db);
        await cart.AddToCartAsync(product.Id, 2);

        // Act
        cart.UpdateQuantity(product.Id, -1);

        // Assert
        Assert.Empty(cart.GetCart());
    }

    [Fact]
    public async Task UpdateQuantity_SanPhamHetHang_KhongDoiQuantity()
    {
        // Arrange — nhánh !item.IsOutOfStock == false
        var db = TestFixtures.CreateContext();
        var product = TestFixtures.SeedActiveProduct(db, stock: 0);
        var (cart, _, _) = TestFixtures.CreateCartEnvironment(db);
        await cart.AddToCartAsync(product.Id, 1);
        Assert.True(Assert.Single(cart.GetCart()).IsOutOfStock);

        // Act
        cart.UpdateQuantity(product.Id, 5);

        // Assert
        Assert.Equal(0, Assert.Single(cart.GetCart()).Quantity);
    }

    [Fact]
    public async Task UpdateQuantity_HopLe_CapNhatSoLuong()
    {
        // Arrange
        var db = TestFixtures.CreateContext();
        var product = TestFixtures.SeedActiveProduct(db, stock: 10);
        var (cart, _, _) = TestFixtures.CreateCartEnvironment(db);
        await cart.AddToCartAsync(product.Id, 1);

        // Act
        cart.UpdateQuantity(product.Id, 4);

        // Assert
        Assert.Equal(4, Assert.Single(cart.GetCart()).Quantity);
    }

    // ---------- Remove / Clear / GetTotal ----------

    [Fact]
    public async Task RemoveItem_XoaDungSanPham()
    {
        // Arrange
        var db = TestFixtures.CreateContext();
        var product = TestFixtures.SeedActiveProduct(db);
        var (cart, _, _) = TestFixtures.CreateCartEnvironment(db);
        await cart.AddToCartAsync(product.Id, 1);

        // Act
        cart.RemoveItem(product.Id);

        // Assert
        Assert.Empty(cart.GetCart());
    }

    [Fact]
    public async Task Clear_XoaToanBoGio()
    {
        // Arrange
        var db = TestFixtures.CreateContext();
        var product = TestFixtures.SeedActiveProduct(db);
        var (cart, _, _) = TestFixtures.CreateCartEnvironment(db);
        await cart.AddToCartAsync(product.Id, 1);

        // Act
        cart.Clear();

        // Assert
        Assert.Empty(cart.GetCart());
    }

    [Fact]
    public async Task GetTotal_KhongTinhSanPhamHetHang()
    {
        // Arrange
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

        // Act
        var total = cart.GetTotal();

        // Assert — chỉ 10 * 2; item hết hàng bị loại
        Assert.Equal(20m, total);
    }

    // ---------- RefreshStockFlagsAsync ----------

    [Fact]
    public async Task RefreshStockFlagsAsync_TonKhoGiamDuoiQuantity_ClampVaGiuHang()
    {
        // Arrange
        var db = TestFixtures.CreateContext();
        var product = TestFixtures.SeedActiveProduct(db, stock: 10);
        var (cart, _, _) = TestFixtures.CreateCartEnvironment(db);
        await cart.AddToCartAsync(product.Id, 5);

        var inventory = await db.ProductInventories.FirstAsync(i => i.ProductId == product.Id);
        inventory.StockQuantity = 2;
        await db.SaveChangesAsync();

        // Act
        await cart.RefreshStockFlagsAsync();

        // Assert
        var item = Assert.Single(cart.GetCart());
        Assert.Equal(2, item.Quantity);
        Assert.False(item.IsOutOfStock);
    }

    [Fact]
    public async Task RefreshStockFlagsAsync_TonKhoVe0_DanhDauHetHang()
    {
        // Arrange
        var db = TestFixtures.CreateContext();
        var product = TestFixtures.SeedActiveProduct(db, stock: 5);
        var (cart, _, _) = TestFixtures.CreateCartEnvironment(db);
        await cart.AddToCartAsync(product.Id, 1);

        var inventory = await db.ProductInventories.FirstAsync(i => i.ProductId == product.Id);
        inventory.StockQuantity = 0;
        await db.SaveChangesAsync();

        // Act
        await cart.RefreshStockFlagsAsync();

        // Assert
        var item = Assert.Single(cart.GetCart());
        Assert.True(item.IsOutOfStock);
        Assert.Equal(0, item.Quantity);
    }

    [Fact]
    public async Task RefreshPricesAsync_PromoMoi_CapNhatGiaTrongGio()
    {
        // Arrange
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

        // Act
        await cart.RefreshPricesAsync();

        // Assert
        var item = Assert.Single(cart.GetCart());
        Assert.Equal(85m, item.UnitPrice);
        Assert.Equal(15m, item.PromoDiscount);
    }
}

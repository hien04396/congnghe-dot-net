using OnlineShop.Models;
using OnlineShop.Services;
using OnlineShop.ViewModels;

namespace OnlineShop.Tests;

public class ProductDetailsViewModelTests
{
    [Fact]
    public void IsOutOfStock_InventoryNull_True()
    {
        var vm = new ProductDetailsViewModel
        {
            Product = new Product { Price = 10m },
            Inventory = null
        };

        Assert.True(vm.IsOutOfStock);
    }

    [Fact]
    public void IsOutOfStock_StockBang0_True()
    {
        var vm = new ProductDetailsViewModel
        {
            Product = new Product { Price = 10m },
            Inventory = new ProductInventory { StockQuantity = 0 }
        };

        Assert.True(vm.IsOutOfStock);
    }

    [Fact]
    public void IsOutOfStock_StockBang1_False()
    {
        var vm = new ProductDetailsViewModel
        {
            Product = new Product { Price = 10m },
            Inventory = new ProductInventory { StockQuantity = 1 }
        };

        Assert.False(vm.IsOutOfStock);
    }

    [Fact]
    public void EffectivePrice_KhongPromo_BangGiaGoc()
    {
        var vm = new ProductDetailsViewModel
        {
            Product = new Product { Price = 49.99m },
            ActivePromo = null
        };

        Assert.Equal(49.99m, vm.EffectivePrice);
    }

    [Fact]
    public void EffectivePrice_CoPromo_GiaTruAmountOff()
    {
        var vm = new ProductDetailsViewModel
        {
            Product = new Product { Price = 100m },
            ActivePromo = new ProductPromo { AmountOff = 25m, IsActive = true }
        };

        Assert.Equal(75m, vm.EffectivePrice);
    }

    [Fact]
    public void EffectivePrice_AmountOffLonHonGia_KhongAm()
    {
        var vm = new ProductDetailsViewModel
        {
            Product = new Product { Price = 10m },
            ActivePromo = new ProductPromo { AmountOff = 50m, IsActive = true }
        };

        Assert.Equal(0m, vm.EffectivePrice);
    }
}

public class ProductListViewModelTests
{
    [Fact]
    public void TotalPages_ChiaHet_DungSoTrang()
    {
        var vm = new ProductListViewModel { TotalCount = 16, PageSize = 8 };
        Assert.Equal(2, vm.TotalPages);
    }

    [Fact]
    public void TotalPages_DuLe_LamTronLen()
    {
        var vm = new ProductListViewModel { TotalCount = 17, PageSize = 8 };
        Assert.Equal(3, vm.TotalPages);
    }

    [Fact]
    public void TotalPages_KhongCoSanPham_Bang0()
    {
        var vm = new ProductListViewModel { TotalCount = 0, PageSize = 8 };
        Assert.Equal(0, vm.TotalPages);
    }
}

public class CartItemTests
{
    [Fact]
    public void HasPromo_DiscountNullHoac0_False()
    {
        Assert.False(new CartItem { PromoDiscount = null }.HasPromo);
        Assert.False(new CartItem { PromoDiscount = 0m }.HasPromo);
    }

    [Fact]
    public void HasPromo_DiscountDuong_True()
    {
        Assert.True(new CartItem { PromoDiscount = 5m }.HasPromo);
    }

    [Fact]
    public void EffectivePrice_BangUnitPrice()
    {
        var item = new CartItem { UnitPrice = 12.5m };
        Assert.Equal(12.5m, item.EffectivePrice);
    }
}

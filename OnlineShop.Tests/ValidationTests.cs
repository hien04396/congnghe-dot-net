using System.ComponentModel.DataAnnotations;
using OnlineShop.Models;
using OnlineShop.ViewModels;

namespace OnlineShop.Tests;

public class ValidationTests
{
    private static IList<ValidationResult> Validate(object model)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(model, new ValidationContext(model), results, validateAllProperties: true);
        return results;
    }

    [Fact]
    public void RegisterViewModel_HopLe_KhongLoi()
    {
        var model = new RegisterViewModel
        {
            Username = "alice",
            Password = "123456",
            ConfirmPassword = "123456"
        };

        var results = Validate(model);

        Assert.Empty(results);
    }

    [Fact]
    public void RegisterViewModel_ThieuUsername_KhongHopLe()
    {
        var model = new RegisterViewModel
        {
            Username = "",
            Password = "123456",
            ConfirmPassword = "123456"
        };

        var results = Validate(model);

        Assert.Contains(results, r => r.MemberNames.Contains(nameof(RegisterViewModel.Username)));
    }

    [Fact]
    public void RegisterViewModel_UsernameDaiHon50_KhongHopLe()
    {
        var model = new RegisterViewModel
        {
            Username = new string('a', 51),
            Password = "123456",
            ConfirmPassword = "123456"
        };

        var results = Validate(model);

        Assert.Contains(results, r => r.MemberNames.Contains(nameof(RegisterViewModel.Username)));
    }

    [Fact]
    public void RegisterViewModel_UsernameDung50_HopLe()
    {
        var model = new RegisterViewModel
        {
            Username = new string('a', 50),
            Password = "123456",
            ConfirmPassword = "123456"
        };

        var results = Validate(model);

        Assert.Empty(results);
    }

    [Fact]
    public void RegisterViewModel_ConfirmPasswordKhongKhop_KhongHopLe()
    {
        var model = new RegisterViewModel
        {
            Username = "alice",
            Password = "123456",
            ConfirmPassword = "654321"
        };

        var results = Validate(model);

        Assert.Contains(results, r => r.ErrorMessage != null && r.ErrorMessage.Contains("do not match"));
    }

    [Fact]
    public void LoginViewModel_ThieuUsernameHoacPassword_KhongHopLe()
    {
        var empty = new LoginViewModel();
        var results = Validate(empty);
        Assert.True(results.Count >= 2);

        var onlyUser = new LoginViewModel { Username = "admin", Password = "" };
        Assert.Contains(Validate(onlyUser), r => r.MemberNames.Contains(nameof(LoginViewModel.Password)));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    public void ProductReview_RatingBienHopLe_Pass(int rating)
    {
        var review = new ProductReview { ProductId = 1, Rating = rating, Title = "OK" };
        Assert.Empty(Validate(review));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    public void ProductReview_RatingNgoaiKhoang_Fail(int rating)
    {
        var review = new ProductReview { ProductId = 1, Rating = rating };

        var results = Validate(review);

        Assert.Contains(results, r => r.MemberNames.Contains(nameof(ProductReview.Rating)));
    }

    [Fact]
    public void Product_ThieuName_KhongHopLe()
    {
        var product = new Product { Name = "", Price = 10m, CategoryId = 1 };
        var results = Validate(product);
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(Product.Name)));
    }

    [Fact]
    public void Product_NameDaiHon200_KhongHopLe()
    {
        var product = new Product { Name = new string('x', 201), Price = 10m, CategoryId = 1 };
        var results = Validate(product);
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(Product.Name)));
    }

    [Fact]
    public void ProductCategory_ThieuName_KhongHopLe()
    {
        var category = new ProductCategory { Name = "" };
        Assert.Contains(Validate(category), r => r.MemberNames.Contains(nameof(ProductCategory.Name)));
    }

    [Fact]
    public void ProductImage_UrlKhongHopLe_Fail()
    {
        var image = new ProductImage { ProductId = 1, ImageUrl = "not-a-url" };
        Assert.NotEmpty(Validate(image));
    }

    [Fact]
    public void ProductImage_UrlHopLe_Pass()
    {
        var image = new ProductImage { ProductId = 1, ImageUrl = "https://example.com/a.png" };
        Assert.Empty(Validate(image));
    }
}

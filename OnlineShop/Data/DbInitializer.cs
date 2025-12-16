using Microsoft.EntityFrameworkCore;
using OnlineShop.Models;

namespace OnlineShop.Data;

public static class DbInitializer
{
    public static void EnsureSeedData(OnlineStoreContext context)
    {
        // Use EnsureCreated instead of Migrate to avoid needing __EFMigrationsHistory table
        context.Database.EnsureCreated();

        if (!context.AdminUsers.Any())
        {
            context.AdminUsers.Add(new AdminUser
            {
                Username = "admin",
                Password = "admin",
                IsDefault = true
            });
        }

        if (!context.ProductCategories.Any())
        {
            var categories = new[]
            {
                new ProductCategory { Name = "Shirts", Description = "Casual and formal shirts" },
                new ProductCategory { Name = "Pants", Description = "Jeans, chinos and more" },
                new ProductCategory { Name = "Jackets", Description = "Light and warm jackets" },
                new ProductCategory { Name = "Dresses", Description = "Casual and party dresses" },
                new ProductCategory { Name = "Skirts", Description = "Skirts for all styles" }
            };

            context.ProductCategories.AddRange(categories);
            context.SaveChanges();

            var shirts = categories[0];
            var pants = categories[1];
            var jackets = categories[2];

            var products = new[]
            {
                new Product
                {
                    Name = "Basic White Shirt",
                    Description = "Simple cotton shirt, perfect for everyday wear.",
                    Price = 19.99m,
                    CategoryId = shirts.Id
                },
                new Product
                {
                    Name = "Blue Denim Jeans",
                    Description = "Classic straight fit denim jeans.",
                    Price = 39.99m,
                    CategoryId = pants.Id
                },
                new Product
                {
                    Name = "Black Hoodie Jacket",
                    Description = "Comfortable hoodie jacket for cool days.",
                    Price = 49.99m,
                    CategoryId = jackets.Id
                }
            };

            context.Products.AddRange(products);
            context.SaveChanges();

            foreach (var product in products)
            {
                context.ProductInventories.Add(new ProductInventory
                {
                    ProductId = product.Id,
                    StockQuantity = 10
                });

                context.ProductImages.Add(new ProductImage
                {
                    ProductId = product.Id,
                    ImageUrl = "/images/sample-placeholder.png",
                    IsPrimary = true
                });
            }
        }

        context.SaveChanges();
    }
}



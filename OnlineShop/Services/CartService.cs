using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Data;

namespace OnlineShop.Services;

public class CartService
{
    private const string CartSessionKey = "CART_ITEMS";
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly OnlineStoreContext _context;

    public CartService(IHttpContextAccessor httpContextAccessor, OnlineStoreContext context)
    {
        _httpContextAccessor = httpContextAccessor;
        _context = context;
    }

    private ISession Session => _httpContextAccessor.HttpContext!.Session;

    public List<CartItem> GetCart()
    {
        var json = Session.GetString(CartSessionKey);
        if (string.IsNullOrEmpty(json))
        {
            return new List<CartItem>();
        }

        return JsonSerializer.Deserialize<List<CartItem>>(json) ?? new List<CartItem>();
    }

    public async Task AddToCartAsync(int productId, int quantity)
    {
        var product = await _context.Products
            .Include(p => p.Inventory)
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == productId && p.IsActive);

        if (product == null)
        {
            return;
        }

        // Get active promo for this product
        var activePromo = await _context.ProductPromos
            .Where(pr => pr.ProductId == productId && pr.IsActive)
            .OrderByDescending(pr => pr.StartDate)
            .FirstOrDefaultAsync();

        var stock = product.Inventory?.StockQuantity ?? 0;
        var cart = GetCart();
        var existing = cart.FirstOrDefault(c => c.ProductId == productId);

        var desiredQty = (existing?.Quantity ?? 0) + quantity;
        var finalQty = Math.Min(desiredQty, stock);

        var isOutOfStock = stock <= 0;
        
        var promoDiscount = activePromo != null ? Math.Min(activePromo.AmountOff, product.Price) : (decimal?)null;
        var effectivePrice = activePromo != null ? Math.Max(0, product.Price - activePromo.AmountOff) : product.Price;

        if (existing == null)
        {
            cart.Add(new CartItem
            {
                ProductId = product.Id,
                ProductName = product.Name,
                OriginalPrice = product.Price,
                UnitPrice = effectivePrice,
                PromoDiscount = promoDiscount,
                Quantity = finalQty,
                IsOutOfStock = isOutOfStock,
                ImageUrl = product.Images.FirstOrDefault(i => i.IsPrimary)?.ImageUrl
            });
        }
        else
        {
            existing.Quantity = finalQty;
            existing.IsOutOfStock = isOutOfStock;
            existing.OriginalPrice = product.Price;
            existing.UnitPrice = effectivePrice;
            existing.PromoDiscount = promoDiscount;
        }

        SaveCart(cart);
    }

    public async Task RefreshStockFlagsAsync()
    {
        var cart = GetCart();
        var ids = cart.Select(c => c.ProductId).ToList();
        var inventories = await _context.ProductInventories
            .Where(i => ids.Contains(i.ProductId))
            .ToDictionaryAsync(i => i.ProductId, i => i.StockQuantity);

        foreach (var item in cart)
        {
            var stock = inventories.TryGetValue(item.ProductId, out var qty) ? qty : 0;
            item.IsOutOfStock = stock <= 0;
            if (item.Quantity > stock)
            {
                item.Quantity = stock;
            }
        }

        SaveCart(cart);
    }

    public async Task RefreshPricesAsync()
    {
        var cart = GetCart();
        var ids = cart.Select(c => c.ProductId).ToList();
        
        // Get current product prices
        var products = await _context.Products
            .Where(p => ids.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id, p => p.Price);
        
        // Get active promos - load all and process in memory
        var activePromosList = await _context.ProductPromos
            .Where(pr => ids.Contains(pr.ProductId) && pr.IsActive)
            .ToListAsync();
        
        // Get the most recent promo for each product
        var activePromos = activePromosList
            .GroupBy(pr => pr.ProductId)
            .ToDictionary(
                g => g.Key, 
                g => g.OrderByDescending(pr => pr.StartDate).First()
            );

        foreach (var item in cart)
        {
            if (products.TryGetValue(item.ProductId, out var price))
            {
                item.OriginalPrice = price;
                
                if (activePromos.TryGetValue(item.ProductId, out var promo))
                {
                    var discount = Math.Min(promo.AmountOff, price);
                    item.PromoDiscount = discount;
                    item.UnitPrice = Math.Max(0, price - promo.AmountOff);
                }
                else
                {
                    item.PromoDiscount = null;
                    item.UnitPrice = price;
                }
            }
        }

        SaveCart(cart);
    }

    public void UpdateQuantity(int productId, int quantity)
    {
        var cart = GetCart();
        var item = cart.FirstOrDefault(c => c.ProductId == productId);
        if (item == null)
        {
            return;
        }

        if (quantity <= 0)
        {
            cart.Remove(item);
        }
        else if (!item.IsOutOfStock)
        {
            item.Quantity = quantity;
        }

        SaveCart(cart);
    }

    public void RemoveItem(int productId)
    {
        var cart = GetCart();
        cart.RemoveAll(c => c.ProductId == productId);
        SaveCart(cart);
    }

    public void Clear()
    {
        Session.Remove(CartSessionKey);
    }

    public decimal GetTotal()
    {
        var cart = GetCart();
        return cart.Where(c => !c.IsOutOfStock).Sum(c => c.UnitPrice * c.Quantity);
    }

    private void SaveCart(List<CartItem> cart)
    {
        var json = JsonSerializer.Serialize(cart);
        Session.SetString(CartSessionKey, json);
    }
}



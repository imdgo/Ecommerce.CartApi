using CartApi.Models;
using CartApi.Repositories;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace CartApi.Services;

public class CartService
{
    private readonly ICartRepository _repository;
    private readonly IDistributedCache _cache;
    private readonly int _cacheExpirationMinutes;

    public CartService(ICartRepository repository, IDistributedCache cache, IConfiguration configuration)
    {
        _repository = repository;
        _cache = cache;

        // Lê o tempo de expiração do appsettings.json
        _cacheExpirationMinutes = configuration.GetValue<int>("Redis:CacheExpirationMinutes");
    }

    private string CacheKey(int cartId) => $"cart_total_{cartId}";

    public virtual async Task<int> CreateCartAsync(string userId)
    {
        return await _repository.CreateCartAsync(userId);
    }

    public virtual async Task<(List<CartItemDto> Items, decimal Total)> GetCartAsync(int cartId)
    {
        var cachedTotal = await _cache.GetStringAsync(CacheKey(cartId));
        var items = await _repository.GetCartItemsWithProductAsync(cartId);

        if (items == null || !items.Any())
            throw new KeyNotFoundException("Cart not found or empty.");

        decimal total;

        if (!string.IsNullOrEmpty(cachedTotal))
        {
            total = decimal.Parse(cachedTotal);
        }
        else
        {
            total = items.Sum(i => i.Quantity * i.Price);
            await _cache.SetStringAsync(
                CacheKey(cartId),
                total.ToString(),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_cacheExpirationMinutes)
                });
        }

        return (items, total);
    }

    public virtual async Task AddItemAsync(CartItem item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));

        var product = await _repository.GetProductByIdAsync(item.ProductId);
        if (product == null)
            throw new KeyNotFoundException("Product not found.");

        await _repository.AddItemAsync(item);

        // Limpa o cache do carrinho após alterações
        await _cache.RemoveAsync(CacheKey(item.CartId));
    }

    public virtual async Task RemoveItemAsync(int cartId, int productId)
    {
        await _repository.RemoveItemAsync(cartId, productId);
        await _cache.RemoveAsync(CacheKey(cartId));
    }

    public virtual async Task ApplyDiscountAsync(Discount discount)
    {
        if (discount == null)
            throw new ArgumentNullException(nameof(discount));

        await _repository.ApplyDiscountAsync(discount);
        await _cache.RemoveAsync(CacheKey(discount.CartId));
    }
}

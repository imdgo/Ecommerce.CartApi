using CartApi.Models;
using CartApi.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CartApi.Tests.Mocks
{
    public class MockCartRepository : ICartRepository
    {
        private readonly Dictionary<int, List<CartItemDto>> _carts = new();

        public Task AddItemAsync(CartItem item)
        {
            if (!_carts.ContainsKey(item.CartId))
                _carts[item.CartId] = new List<CartItemDto>();

            var existingItem = _carts[item.CartId].FirstOrDefault(i => i.ProductId == item.ProductId);
            if (existingItem != null)
                existingItem.Quantity += item.Quantity;
            else
                _carts[item.CartId].Add(new CartItemDto
                {
                    CartId = item.CartId,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Name = $"Product {item.ProductId}",
                    Price = 10m
                });

            return Task.CompletedTask;
        }

        public Task RemoveItemAsync(int cartId, int productId)
        {
            if (_carts.ContainsKey(cartId))
            {
                var item = _carts[cartId].FirstOrDefault(i => i.ProductId == productId);
                if (item != null)
                    _carts[cartId].Remove(item);
            }
            return Task.CompletedTask;
        }

        public Task<List<CartItemDto>> GetCartItemsWithProductAsync(int cartId)
        {
            var items = _carts.ContainsKey(cartId) ? _carts[cartId] : new List<CartItemDto>();
            return Task.FromResult(items);
        }

        public Task ApplyDiscountAsync(Discount discount)
        {
            if (!_carts.ContainsKey(discount.CartId))
                return Task.CompletedTask;

            foreach (var item in _carts[discount.CartId])
            {
                item.Price = item.Price * (1 - discount.Percentual);
            }

            return Task.CompletedTask;
        }

        public Task<int> CreateCartAsync(string userId)
        {
            int newCartId = _carts.Count + 1;
            _carts[newCartId] = new List<CartItemDto>();
            return Task.FromResult(newCartId);
        }

        public Task<Product> GetProductByIdAsync(int productId)
        {
            return Task.FromResult(new Product
            {
                ProductId = productId,
                Name = $"Product {productId}",
                Price = 10m
            });
        }
    }
}

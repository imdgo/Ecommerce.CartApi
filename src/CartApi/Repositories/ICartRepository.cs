using CartApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CartApi.Repositories
{
    public interface ICartRepository
    {
        Task<int> CreateCartAsync(string userId);
        Task<List<CartItemDto>> GetCartItemsWithProductAsync(int cartId);
        Task AddItemAsync(CartItem item);
        Task RemoveItemAsync(int cartId, int productId);
        Task ApplyDiscountAsync(Discount discount);
        Task<Product> GetProductByIdAsync(int productId);
    }
}


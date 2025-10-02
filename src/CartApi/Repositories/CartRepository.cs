using CartApi.Models;
using Dapper;
using System.Data;

namespace CartApi.Repositories;

public class CartRepository : ICartRepository
{
    private readonly IDbConnection _db;

    public CartRepository(IDbConnection db)
    {
        _db = db;
    }

    public async Task<int> CreateCartAsync(string userId)
    {
        var query = "INSERT INTO Cart (UserId) OUTPUT INSERTED.CartId VALUES (@UserId)";
        return await _db.QuerySingleAsync<int>(query, new { UserId = userId });
    }

    public async Task<List<CartItem>> GetItemsAsync(int cartId)
    {
        var query = "SELECT CartItemId, CartId, ProductId, Quantity FROM CartItem WHERE CartId = @CartId";
        var items = await _db.QueryAsync<CartItem>(query, new { CartId = cartId });
        return items.ToList();
    }

    public async Task<List<CartItemDto>> GetCartItemsWithProductAsync(int cartId)
    {
        var query = @"
            SELECT ci.CartItemId, ci.CartId, ci.ProductId, ci.Quantity, p.Price, p.Name
            FROM CartItem ci
            INNER JOIN Product p ON ci.ProductId = p.ProductId
            WHERE ci.CartId = @CartId";
        var result = await _db.QueryAsync<CartItemDto>(query, new { CartId = cartId });
        return result.ToList();
    }

    public async Task AddItemAsync(CartItem item)
    {
        var product = await GetProductByIdAsync(item.ProductId);
        var query = @"INSERT INTO CartItem (CartId, ProductId, Quantity)
                      VALUES (@CartId, @ProductId, @Quantity)";
        await _db.ExecuteAsync(query, item);
    }

    public async Task RemoveItemAsync(int cartId, int productId)
    {
        var query = @"DELETE FROM CartItem WHERE CartId = @CartId AND ProductId = @ProductId";
        await _db.ExecuteAsync(query, new { CartId = cartId, ProductId = productId });
    }

    public async Task ApplyDiscountAsync(Discount discount)
    {
        var query = @"INSERT INTO Discount (CartId, Percentual) VALUES (@CartId, @Percentual)";
        await _db.ExecuteAsync(query, discount);
    }

    public async Task<Product> GetProductByIdAsync(int productId)
    {
        var query = "SELECT * FROM Product WHERE ProductId = @ProductId";
        var product = await _db.QueryFirstOrDefaultAsync<Product>(query, new { ProductId = productId });
        if (product == null)
            throw new InvalidOperationException($"Product with ID {productId} not found.");
        return product;
    }
}

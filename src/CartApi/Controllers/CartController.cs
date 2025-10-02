using Microsoft.AspNetCore.Mvc;
using CartApi.Models;
using CartApi.Services;

namespace CartApi.Controllers;

[ApiController]
[Route("carts")]
public class CartController : ControllerBase
{
    private readonly CartService _service;

    public CartController(CartService service)
    {
        _service = service;
    }

    // POST /carts
    [HttpPost]
    public async Task<IActionResult> CreateCart([FromBody] string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return BadRequest(new { message = "UserId is required." });

        var cartId = await _service.CreateCartAsync(userId);
        return Ok(new { CartId = cartId });
    }

    // GET /carts/{cartId}
    [HttpGet("{cartId}")]
    public async Task<IActionResult> GetCart(int cartId)
    {
        try
        {
            var cart = await _service.GetCartAsync(cartId);
            return Ok(cart);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    // POST /carts/{cartId}/items
    [HttpPost("{cartId}/items")]
    public async Task<IActionResult> AddItem(int cartId, [FromBody] CartItem item)
    {
        if (item == null || item.ProductId <= 0 || item.Quantity <= 0)
            return BadRequest(new { message = "Invalid item data." });

        try
        {
            item.CartId = cartId;
            await _service.AddItemAsync(item);
            return Ok(new { message = "Item added successfully." });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    // DELETE /carts/{cartId}/items/{productId}
    [HttpDelete("{cartId}/items/{productId}")]
    public async Task<IActionResult> RemoveItem(int cartId, int productId)
    {
        try
        {
            await _service.RemoveItemAsync(cartId, productId);
            return Ok(new { message = "Item removed successfully." });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // POST /carts/{cartId}/discount
    [HttpPost("{cartId}/discount")]
    public async Task<IActionResult> ApplyDiscount(int cartId, [FromBody] Discount discount)
    {
        if (discount == null || discount.Percentual <= 0)
            return BadRequest(new { message = "Invalid discount data." });

        try
        {
            discount.CartId = cartId;
            await _service.ApplyDiscountAsync(discount);
            return Ok(new { message = "Discount applied successfully." });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
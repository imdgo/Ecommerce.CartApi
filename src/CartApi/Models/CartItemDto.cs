namespace CartApi.Models;

public class CartItemDto : CartItem
{
    public decimal Price { get; set; }
    public string Name { get; set; } = string.Empty;
}
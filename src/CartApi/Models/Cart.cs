namespace CartApi.Models;

public class Cart
{
    public int CartId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public List<CartItem> Itens { get; set; } = [];
    public decimal Total { get; set; }
}

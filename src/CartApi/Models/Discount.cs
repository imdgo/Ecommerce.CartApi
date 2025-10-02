namespace CartApi.Models;

public class Discount
{
    public int DiscountId { get; set; }
    public int CartId { get; set; }
    public decimal Percentual { get; set; }
}

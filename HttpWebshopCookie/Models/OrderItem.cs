namespace HttpWebshopCookie.Models;

public class OrderItem
{
    public Order? Order { get; set; }
    public string OrderId { get; set; } = null!;
    public Product? ProductItem { get; set; }
    public string ProductId { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
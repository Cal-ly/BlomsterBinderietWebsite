namespace HttpWebshopCookie.Models;

public class OrderItem
{
    public Order? Order { get; set; }
    public string? OrderId { get; set; }
    public Product? ProductItem { get; set; }
    public string? ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
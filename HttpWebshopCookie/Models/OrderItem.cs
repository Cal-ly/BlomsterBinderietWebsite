namespace HttpWebshopCookie.Models;

public class OrderItem
{
    [ForeignKey("OrderId")]
    public Order? Order { get; set; }
    public string? OrderId { get; set; }

    [ForeignKey("ProductId")]
    public Product? ProductItem { get; set; }
    public string? ProductId { get; set; }

    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
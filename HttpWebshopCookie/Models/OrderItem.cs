namespace HttpWebshopCookie.Models;

public class OrderItem
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [ForeignKey("OrderId")]
    public Order Order { get; set; }
    public string OrderId { get; set; }

    [ForeignKey("ProductId")]
    public Product Product { get; set; }
    public string ProductId { get; set; }

    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
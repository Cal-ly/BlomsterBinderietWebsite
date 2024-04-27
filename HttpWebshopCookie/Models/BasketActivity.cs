namespace HttpWebshopCookie.Models;

public class BasketActivity
{
    public string Id { get; } = Guid.NewGuid().ToString();
    public string? ActivityType { get; set; } // Types like "Add", "Remove", "Update", etc.
    public int? QuantityChanged { get; set; } //Pos numbers for add, neg numbers for remove
    public string? UserId { get; set; }
    public bool IsRegisteredUser { get; set; }
    public string? SessionId { get; set; }
    public string? BasketId { get; set; }
    public Basket? Basket { get; set; }
    public string? ProductId { get; set; }
    public Product? Product { get; set; }
    public DateTime? Timestamp { get; set; }
}
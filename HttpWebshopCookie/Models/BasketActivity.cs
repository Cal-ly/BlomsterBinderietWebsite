namespace HttpWebshopCookie.Models;

public class BasketActivity
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id { get; } = Guid.NewGuid().ToString();

    [ForeignKey("BasketId")]
    public string? BasketId { get; set; }
    public Basket? Basket { get; set; }

    [ForeignKey("ProductId")]
    public string? ProductId { get; set; }
    public Product? Product { get; set; }

    [ForeignKey("UserId")]
    public string? UserId { get; set; }
    public IdentityUser? User { get; set; }

    public string? SessionId { get; set; }
    public string? ActivityType { get; set; } // Types like "Add", "Remove", "Update", etc.
    public int? QuantityChanged { get; set; } //Pos numbers for add, neg numbers for remove
    public DateTime? Timestamp { get; private set; } = DateTime.UtcNow;
}
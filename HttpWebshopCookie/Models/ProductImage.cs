namespace HttpWebshopCookie.Models;

public class ProductImage
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string? ProductId { get; set; }
    public virtual Product? Product { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsPrimary { get; set; } = false;
}

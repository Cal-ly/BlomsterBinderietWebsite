namespace HttpWebshopCookie.Models;

public class Product
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; } = 0;
    public bool IsDeleted { get; set; }
    public string? ImageUrl { get; set; } = "/images/products/default.jpg";
    public DateTime? UpdatedAt { get; set; }
    public virtual ICollection<IXProductTag> ProductTags { get; set; } = [];
    public string GetPriceString()
    {
        return Price.ToString("C2");
    }
}
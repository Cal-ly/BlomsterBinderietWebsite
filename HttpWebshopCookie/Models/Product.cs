using System.ComponentModel.DataAnnotations;
using HttpWebshopCookie.Models.IndexTables;

namespace HttpWebshopCookie.Models;

public class Product
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; } = 0;
    public bool IsDeleted { get; set; }
    public string? ImageUrl { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public virtual ICollection<ProductTag> ProductTags { get; set; } = [];
}
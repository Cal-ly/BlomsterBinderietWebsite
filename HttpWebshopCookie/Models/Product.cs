using System.ComponentModel.DataAnnotations;
using HttpWebshopCookie.Models.IndexTables;

namespace HttpWebshopCookie.Models;

public class Product
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; } = 0;
    public string? ImageUrl { get; set; }
    public bool IsActivated { get; set; } = true;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
    public virtual ICollection<ProductTag> ProductTags { get; set; } = [];
}
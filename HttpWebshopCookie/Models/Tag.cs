using HttpWebshopCookie.Models.IndexTables;

namespace HttpWebshopCookie.Models;

public class Tag
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string? Catergory { get; set; }
    public string? SubCategory { get; set; }
    // Navigation property for the join table
    public List<ProductTag> ProductTags { get; set; } = new List<ProductTag>();
}

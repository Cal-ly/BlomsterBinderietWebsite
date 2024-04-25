using HttpWebshopCookie.Models.IndexTables;

namespace HttpWebshopCookie.Models;

public class Tag
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string? Name { get; set; }
    public string? Catergory { get; set; }
    public string? SubCategory { get; set; }
    public List<ProductTag> ProductTags { get; set; } = [];
}

//TODO: Use Enum for Catergory and SubCategory
//This is a flower arrangement shop, so the catergory could be "Wedding" and the subcategory could be "Bridal Bouquet"
//Ideas: Enum for Catergory: Wedding, Funeral, Birthday, Anniversary, Graduation, Baby Shower, Bridal Shower, etc.
//Ideas: Enum for SubCategory: Bridal Bouquet, Bridesmaid Bouquet, Boutonniere, Corsage, Centerpiece, etc.
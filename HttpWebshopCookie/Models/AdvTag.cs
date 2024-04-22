namespace HttpWebshopCookie.Models;

public class AdvTag
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string? Catergory { get; set; }
    public string? SubCategory { get; set; }
    // Navigation property for the join table
    public List<AdvProductTag> AdvProductTags { get; set; } = new List<AdvProductTag>();
}

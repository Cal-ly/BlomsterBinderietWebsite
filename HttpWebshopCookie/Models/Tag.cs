namespace HttpWebshopCookie.Models;

public class Tag
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string? Occasion { get; set; }
    public string? Category { get; set; }
    public string? SubCategory { get; set; }
    public List<IXProductTag> ProductTags { get; set; } = [];
}
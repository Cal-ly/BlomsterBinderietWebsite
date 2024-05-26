namespace HttpWebshopCookie.ViewModels;

public class ProductViewModel
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public string? Price { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public List<TagViewModel> Tags { get; set; } = [];
}
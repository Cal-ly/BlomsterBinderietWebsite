namespace HttpWebshopCookie.Models.IndexTables;

public class ProductTag
{
    public Product? Product { get; set; }
    public string? ProductId { get; set; }
    public Tag? Tag { get; set; }
    public string? TagId { get; set; }
}
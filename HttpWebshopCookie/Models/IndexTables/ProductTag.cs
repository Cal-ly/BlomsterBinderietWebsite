namespace HttpWebshopCookie.Models.IndexTables;

public class ProductTag
{
    [ForeignKey("ProductId")]
    public Product? Product { get; set; }
    public string? ProductId { get; set; }
    [ForeignKey("TagId")]
    public Tag? Tag { get; set; }
    public string? TagId { get; set; }
}

namespace HttpWebshopCookie.Data.IndexTables;

public class ProductTag
{
    public virtual Product? Product { get; set; }
    public string? ProductId { get; set; }
    public virtual Tag? Tag { get; set; }
    public string? TagId { get; set; }
}
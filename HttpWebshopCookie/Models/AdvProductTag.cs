namespace HttpWebshopCookie.Models;

public class AdvProductTag
{
    public string? AdvProductId { get; set; }
    public AdvProduct? AdvProduct { get; set; }

    public string? AdvTagId { get; set; }
    public AdvTag? AdvTag { get; set; }
}

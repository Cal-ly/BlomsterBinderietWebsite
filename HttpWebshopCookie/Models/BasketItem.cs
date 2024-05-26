namespace HttpWebshopCookie.Models;

public class BasketItem
{
    public Basket? Basket { get; set; }
    public string? BasketId { get; set; }
    public Product? ProductInBasket { get; set; }
    public string? ProductId { get; set; }
    public int? Quantity { get; set; }
    public decimal LinePrice()
    {
        return ProductInBasket?.Price * Quantity ?? 0;
    }
}
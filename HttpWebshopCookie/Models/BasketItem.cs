namespace HttpWebshopCookie.Models;

public class BasketItem
{
    [ForeignKey("BasketId")]
    public Basket? Basket { get; set; }
    public string? BasketId { get; set; }

    [ForeignKey("ProductId")]
    public Product? ProductInBasket { get; set; }
    public string? ProductId { get; set; }
    public int? Quantity { get; set; }

    public decimal LinePrice()
    {
        return ProductInBasket?.Price * Quantity ?? 0;
    }
}
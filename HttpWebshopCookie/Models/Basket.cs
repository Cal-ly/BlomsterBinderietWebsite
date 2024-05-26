namespace HttpWebshopCookie.Models;

public class Basket
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public ICollection<BasketItem> Items { get; set; } = [];
    public decimal TotalPrice()
    {
        return Items.Sum(item => item.LinePrice());
    }
    public int TotalItems()
    {
        return Items.Sum(item => item.Quantity) ?? 0;
    }
}
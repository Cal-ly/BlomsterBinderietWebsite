namespace HttpWebshopCookie.Models;

public class Basket
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public List<BasketItem> Items { get; set; } = new List<BasketItem>();
    public decimal TotalPrice()
    {
        return Items.Sum(item => item.LinePrice());
    }
}
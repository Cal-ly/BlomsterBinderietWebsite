namespace HttpWebshopCookie.Models;

public class Order
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string? UserId { get; set; }  // Assuming there is a User class/model
    public DateTime OrderDate { get; set; } = DateTime.Now;
    public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
namespace HttpWebshopCookie.Models.Users;

public class GuestUser
{
    public string Id { get; set; } = Guid.NewGuid().ToString(); // Unique identifier
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public virtual Address? ShippingAddress { get; set; }
    public virtual ICollection<Order> Orders { get; set; } = [];
}

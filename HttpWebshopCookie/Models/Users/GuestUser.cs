namespace HttpWebshopCookie.Models.Users;

public class GuestUser
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id { get; set; } = Guid.NewGuid().ToString(); // Unique identifier
    public string? Email { get; set; }
    public string? Phone { get; set; }

    [ForeignKey("AddressId")]
    public virtual Address? Address { get; set; }
    public string? AddressId { get; set; }
    public virtual ICollection<Order> Orders { get; set; } = [];
}

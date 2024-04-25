namespace HttpWebshopCookie.Models.Users;

public class Guest
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id { get; set; } = Guid.NewGuid().ToString(); // Unique identifier
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }

    [ForeignKey("AddressId")]
    public virtual Address? Address { get; set; }
    public string? AddressId { get; set; }
    public virtual ICollection<Order> Orders { get; set; } = [];
}

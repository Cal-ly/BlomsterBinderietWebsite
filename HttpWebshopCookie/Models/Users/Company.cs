namespace HttpWebshopCookie.Models.Users;

public class Company
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string? Name { get; set; }

    [ForeignKey("AddressId")]
    public virtual Address? Address { get; set; }
    public string? AddressId { get; set; }
    public virtual ICollection<Customer> Customers { get; set; } = [];
}
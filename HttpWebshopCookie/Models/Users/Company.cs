namespace HttpWebshopCookie.Models.Users;

public class Company
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string? Name { get; set; }
    public virtual Address? Address { get; set; }
    public virtual ICollection<Customer> Customers { get; set; } = [];
}
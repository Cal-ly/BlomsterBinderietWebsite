namespace HttpWebshopCookie.Models.Users;

public class Company
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string? CVR { get; set; }
    public string? Name { get; set; }
    public string? PhoneNumber { get; set; }
    public virtual Address? Address { get; set; }
    public string? AddressId { get; set; }
    public virtual ICollection<Customer> Representatives { get; set; } = [];
}
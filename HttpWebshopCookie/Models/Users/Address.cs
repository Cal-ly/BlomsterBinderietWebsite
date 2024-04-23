namespace HttpWebshopCookie.Models.Users;

public class Address
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string? Id { get; set; } = Guid.NewGuid().ToString();
    public string? Resident { get; set; }
    public string? Street { get; set; }
    public string? City { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
}

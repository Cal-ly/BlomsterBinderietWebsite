namespace HttpWebshopCookie.Models.Users;

public class Customer : ApplicationUser
{
    public string? Title { get; set; }
    public DateTime? BirthDate { get; set; }
    public virtual Company? Company { get; set; }
    public string? CompanyId { get; set; }
}

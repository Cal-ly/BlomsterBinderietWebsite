namespace HttpWebshopCookie.Models.Users;

public class Customer : ApplicationUser
{
    public string? Title { get; set; }
    public DateTime? BirthDate { get; set; }
    public DateTime? RegistrationDate { get; set; }
    public DateTime? LastLogin { get; set; }

    [ForeignKey("CompanyId")]
    public virtual Company? Company { get; set; }
    public string? CompanyId { get; set; }
    public virtual ICollection<Order> Orders { get; set; } = [];
}

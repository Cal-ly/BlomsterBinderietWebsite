namespace HttpWebshopCookie.Models.Users;

public class Employee : ApplicationUser
{
    public string? JobTitle { get; set; }
    public decimal? Salary { get; set; }
    public DateTime? TerminationDate { get; set; }
}

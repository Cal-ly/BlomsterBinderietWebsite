namespace HttpWebshopCookie.Models;

public class Order
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public DateTime OrderDate { get; set; } = DateTime.Now;
    public DateTime? CompletionDate { get; set; }
    public OrderStatus Status { get; set; }

    [ForeignKey("EmployeeId")]
    public Employee? Employee { get; set; }
    public string? EmployeeId { get; set; }

    [ForeignKey("CustomerId")]
    public Customer? Customer { get; set; }
    public string? CustomerId { get; set; }

    [ForeignKey("GuestUserId")]
    public Guest? GuestUser { get; set; }
    public string? GuestUserId { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; } = [];
}

public enum OrderStatus
{
    [Display(Name = "Pending")]
    Pending = 0,
    [Display(Name = "Received")]
    Received = 1,
    [Display(Name = "Accepted")]
    Accepted = 2,
    [Display(Name = "Processing")]
    Processing = 3,
    [Display(Name = "Ready")]
    Ready = 4,
    [Display(Name = "Delivered")]
    Delivered = 5,
    [Display(Name = "Cancelled")]
    Cancelled = 6
}
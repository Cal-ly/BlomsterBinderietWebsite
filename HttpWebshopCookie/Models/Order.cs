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
    public GuestUser? GuestUser { get; set; }
    public string? GuestUserId { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; } = [];
}

public enum OrderStatus
{
    [Display(Name = "Received")]
    Received = 1,
    [Display(Name = "Processing")]
    Processing = 2,
    [Display(Name = "Ready")]
    Ready = 3,
    [Display(Name = "Delivered")]
    Delivered = 4,
    [Display(Name = "Cancelled")]
    Cancelled = 5
}
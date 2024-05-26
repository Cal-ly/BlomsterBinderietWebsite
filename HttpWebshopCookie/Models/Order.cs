namespace HttpWebshopCookie.Models;

public class Order
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public DateTime OrderDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    public OrderStatus Status { get; set; }
    public Customer? Customer { get; set; }
    public string? CustomerId { get; set; }
    public Guest? Guest { get; set; }
    public string? GuestId { get; set; }
    public Employee? Employee { get; set; }
    public string? EmployeeId { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public string? SpecialOrderInstructionsId { get; set; }
    public SpecialOrderInstruction? SpecialOrderInstruction { get; set; }

    public decimal TotalPrice
    {
        get { return GetTotalPrice(); }
        private set { }
    }

    public decimal GetTotalPrice()
    {
        decimal totalPrice = OrderItems?.Sum(item => item.UnitPrice * item.Quantity) ?? 0;
        if (SpecialOrderInstruction != null)
        {
            if (SpecialOrderInstruction.Delivery)
            {
                totalPrice += 500;
            }
            if (SpecialOrderInstruction.Arrangement)
            {
                totalPrice += 500;
            }
        }
        return totalPrice;
    }
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
    [Display(Name = "Completed")]
    Completed = 6,
    [Display(Name = "Cancelled")]
    Cancelled = 7
}
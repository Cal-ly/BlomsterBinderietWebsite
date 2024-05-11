namespace HttpWebshopCookie.Pages.Orders;

public class OrderListingsModel(ApplicationDbContext context) : PageModel
{
    public List<Order> Orders { get; set; } = new List<Order>();
    public OrderStatus? FilterStatus { get; set; }
    public string? CustomerName { get; set; }
    public string? EmployeeName { get; set; }
    [BindProperty(SupportsGet = true)]
    public DateTime? StartDate { get; set; }
    [BindProperty(SupportsGet = true)]
    public DateTime? EndDate { get; set; }

    //TODO - colormarking to show status
    public async Task OnGetAsync(OrderStatus? filterStatus, string? customerName, string? employeeName)
    {
        var ordersQuery = context.Orders
            .Include(o => o.Customer)
            .Include(o => o.Guest)
            .Include(o => o.Employee)
            .Include(o => o.OrderItems)
            .AsQueryable();

        if (filterStatus.HasValue)
        {
            ordersQuery = ordersQuery.Where(o => o.Status == filterStatus);
        }

        if (!string.IsNullOrEmpty(customerName))
        {
            ordersQuery = ordersQuery.Where(o =>
                (o.Customer != null && (o.Customer.FirstName + " " + o.Customer.LastName).Contains(customerName)) ||
                (o.Guest != null && (o.Guest.FirstName + " " + o.Guest.LastName).Contains(customerName)));
        }

        if (StartDate.HasValue)
        {
            ordersQuery = ordersQuery.Where(o => o.OrderDate >= StartDate.Value);
        }
        if (EndDate.HasValue)
        {
            ordersQuery = ordersQuery.Where(o => o.OrderDate <= EndDate.Value);
        }

        if (!string.IsNullOrEmpty(employeeName))
        {
            ordersQuery = ordersQuery.Where(o => o.Employee != null &&
                                                (o.Employee.FirstName + " " + o.Employee.LastName)
                                                .Contains(employeeName));
        }

        Orders = await ordersQuery.OrderByDescending(o => o.OrderDate).ToListAsync();
    }

    public IActionResult OnPostSearch(OrderStatus? filterStatus, string? customerName, string? employeeName)
    {
        return RedirectToPage(new { filterStatus, customerName, employeeName });
    }
}
namespace HttpWebshopCookie.Pages.Orders;

public class OrderListingsModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public OrderListingsModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<Order> Orders { get; set; } = new List<Order>();
    public OrderStatus? FilterStatus { get; set; }
    public string? CustomerName { get; set; }

    public async Task OnGetAsync(OrderStatus? filterStatus, string? customerName)
    {
        var ordersQuery = _context.Orders.Include(o => o.Customer).Include(o => o.OrderItems).AsQueryable();

        if (filterStatus.HasValue)
        {
            ordersQuery = ordersQuery.Where(o => o.Status == filterStatus);
        }

        if (!string.IsNullOrEmpty(customerName))
        {
            ordersQuery = ordersQuery.Where(o => o.Customer != null &&
                                                (o.Customer.FirstName + " " + o.Customer.LastName)
                                                 .Contains(customerName));
        }

        Orders = await ordersQuery.OrderByDescending(o => o.OrderDate).ToListAsync();
    }

    public IActionResult OnPostSearch(OrderStatus? filterStatus, string? customerName)
    {
        return RedirectToPage(new { filterStatus, customerName });
    }
}

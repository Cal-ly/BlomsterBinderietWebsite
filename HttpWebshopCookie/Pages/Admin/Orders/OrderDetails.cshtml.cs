namespace HttpWebshopCookie.Pages.Orders;

public class OrderDetailsModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public OrderDetailsModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public Order? Order { get; set; }
    public List<Employee> Employees { get; set; } = new List<Employee>();
    public string? Message { get; set; }

    public async Task<IActionResult> OnGetAsync(string id)
    {
        Order = await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.Employee)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.ProductItem)
            .Include(o => o.SpecialOrderInstruction)
            .ThenInclude(soi => soi!.SpecialDeliveryAddress)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (Order == null)
        {
            Message = "Order not found";
            return Page();
        }

        if (User.IsInRole("manager") || User.IsInRole("admin"))
        {
            Employees = await _context.Employees.ToListAsync();
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAssignToSelfAsync(string id)
    {
        var order = await _context.Orders.FindAsync(id);

        if (order == null)
        {
            Message = "Order not found";
            return Page();
        }

        var employeeId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(employeeId))
        {
            Message = "User not authorized";
            return Page();
        }

        order.EmployeeId = employeeId;

        await _context.SaveChangesAsync();

        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostAssignToEmployeeAsync(string id, string employeeId)
    {
        if (!User.IsInRole("manager") && !User.IsInRole("admin"))
        {
            Message = "Not authorized";
            return Page();
        }

        var order = await _context.Orders.FindAsync(id);

        if (order == null)
        {
            Message = "Order not found";
            return Page();
        }

        order.EmployeeId = employeeId;

        await _context.SaveChangesAsync();

        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostUpdateStatusAsync(string id, OrderStatus status)
    {
        var order = await _context.Orders.FindAsync(id);

        if (order == null)
        {
            Message = "Order not found";
            return Page();
        }

        order.Status = status;
        await _context.SaveChangesAsync();

        return RedirectToPage(new { id });
    }
}

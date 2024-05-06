using System.Security.Claims;

namespace HttpWebshopCookie.Pages.Orders;

public class OrderDetailsModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public OrderDetailsModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public Order? Order { get; set; }
    public string? Message { get; set; }

    public async Task<IActionResult> OnGetAsync(string id)
    {
        Order = await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.Employee)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.ProductItem)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (Order == null)
        {
            Message = "Order not found";
            return Page();
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
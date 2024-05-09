namespace HttpWebshopCookie.Pages.Orders;

public class OrderDetailsModel(ApplicationDbContext context) : PageModel
{
    public Order? Order { get; set; }
    public string? Message { get; set; }

    public async Task<IActionResult> OnGetAsync(string id)
    {
        Order = await context.Orders
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
        var order = await context.Orders.FindAsync(id);

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

        await context.SaveChangesAsync();

        return RedirectToPage(new { id });
    }

    public async Task<IActionResult> OnPostUpdateStatusAsync(string id, OrderStatus status)
    {
        var order = await context.Orders.FindAsync(id);

        if (order == null)
        {
            Message = "Order not found";
            return Page();
        }

        order.Status = status;
        await context.SaveChangesAsync();

        return RedirectToPage(new { id });
    }
}
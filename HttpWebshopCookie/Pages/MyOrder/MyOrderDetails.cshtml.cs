namespace HttpWebshopCookie.Pages.MyOrder;

[Authorize]
public class MyOrderDetailsModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<Customer> _userManager;

    public MyOrderDetailsModel(ApplicationDbContext context, UserManager<Customer> userManager)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
    }

    public Order Order { get; set; } = new Order();

    public async Task<IActionResult> OnGetAsync(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return NotFound();
        }
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            //TODO Handle user not found
            return NotFound();
        }

        Order? orderToGet = await GetOrderForUserAsync(user, id);
        
        if (orderToGet == null)
        {
            return NotFound();
        }
        Order = orderToGet;
        return Page();
    }

    private async Task<Order?> GetOrderForUserAsync(Customer user, string orderId)
    {
        if (await _userManager.IsInRoleAsync(user, "companyrep"))
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.ProductItem)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.Customer!.CompanyId == user.CompanyId);
        }
        else
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.ProductItem)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.CustomerId == user.Id);
        }
    }
}

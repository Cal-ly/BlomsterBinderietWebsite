namespace HttpWebshopCookie.Pages.MyOrder;

[Authorize]
public class MyOrderListingModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<Customer> _userManager;

    public MyOrderListingModel(ApplicationDbContext context, UserManager<Customer> userManager)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
    }

    public List<Order> Orders { get; set; } = new List<Order>();

    public async Task OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return;
        }

        Orders = await GetOrdersForUserAsync(user);
    }

    private async Task<List<Order>> GetOrdersForUserAsync(Customer user)
    {
        if (await _userManager.IsInRoleAsync(user, "companyrep"))
        {
            return await _context.Orders
                .Where(o => o.Customer!.CompanyId == user.CompanyId)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.ProductItem)
                .ToListAsync();
        }
        else
        {
            return await _context.Orders
                .Where(o => o.CustomerId == user.Id)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.ProductItem)
                .ToListAsync();
        }
    }
}

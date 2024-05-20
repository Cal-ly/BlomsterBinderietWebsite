namespace HttpWebshopCookie.Pages.Admin.Customers;

[Authorize(Policy = "managerAccess")]
public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<Customer> Customers { get; set; } = new List<Customer>();

    public async Task OnGetAsync()
    {
        Customers = await _context.Users.OfType<Customer>()
                                        .Include(c => c.Company)
                                        .ToListAsync();
    }
}

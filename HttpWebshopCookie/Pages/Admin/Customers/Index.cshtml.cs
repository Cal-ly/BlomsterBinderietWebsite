namespace HttpWebshopCookie.Pages.Admin.Customers;

[Authorize(Policy = "managerAccess")]
public class IndexModel : PageModel
{
    private readonly UserManager<Customer> _userManager;

    public IndexModel(UserManager<Customer> userManager)
    {
        _userManager = userManager;
    }

    public List<Customer> Customers { get; set; } = new();

    public async Task OnGetAsync()
    {
        Customers =
        [
            .. await _userManager.GetUsersInRoleAsync("customer"),
            .. await _userManager.GetUsersInRoleAsync("companyrep"),
        ];
    }
}

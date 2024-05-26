namespace HttpWebshopCookie.Pages.Admin.Customers;

[Authorize(Policy = "managerAccess")]
public class DetailsModel : PageModel
{
    private readonly UserManager<Customer> _userManager;

    public DetailsModel(UserManager<Customer> userManager)
    {
        _userManager = userManager;
    }

    public Customer CustomerDetails { get; set; } = new Customer();

    public async Task<IActionResult> OnGetAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        CustomerDetails = user;
        return Page();
    }
}
namespace HttpWebshopCookie.Pages.Admin.Customers;

[Authorize(Policy = "managerAccess")]
public class DeleteModel : PageModel
{
    private readonly UserManager<Customer> _userManager;

    public DeleteModel(UserManager<Customer> userManager)
    {
        _userManager = userManager;
    }

    [BindProperty]
    public Customer CustomerToDelete { get; set; } = new Customer();

    public async Task<IActionResult> OnGetAsync(string id)
    {
        Customer? customer = await _userManager.FindByIdAsync(id);
        if (customer == null)
        {
            return NotFound();
        }
        CustomerToDelete = customer;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (CustomerToDelete == null)
        {
            return NotFound();
        }

        var result = await _userManager.DeleteAsync(CustomerToDelete);
        if (result.Succeeded)
        {
            return RedirectToPage("Index");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return Page();
    }
}
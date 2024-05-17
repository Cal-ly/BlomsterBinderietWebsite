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
    public Customer UserToDelete { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(string id)
    {
        Customer? userToDelete = await _userManager.FindByIdAsync(id);
        if (userToDelete == null)
        {
            return NotFound();
        }
        UserToDelete = userToDelete;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (UserToDelete == null)
        {
            return NotFound();
        }

        var result = await _userManager.DeleteAsync(UserToDelete);
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
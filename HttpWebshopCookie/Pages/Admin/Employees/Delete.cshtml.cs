namespace HttpWebshopCookie.Pages.Admin.Employees;

[Authorize(Policy = "managerAccess")]
public class DeleteModel : PageModel
{
    private readonly UserManager<Employee> _userManager;

    public DeleteModel(UserManager<Employee> userManager)
    {
        _userManager = userManager;
    }

    [BindProperty]
    public Employee UserToDelete { get; set; }

    public async Task<IActionResult> OnGetAsync(string id)
    {
        UserToDelete = await _userManager.FindByIdAsync(id);
        if (UserToDelete == null)
        {
            return NotFound();
        }
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
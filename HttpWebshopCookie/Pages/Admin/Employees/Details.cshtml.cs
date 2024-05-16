namespace HttpWebshopCookie.Pages.Admin.Employees;

[Authorize(Policy = "managerAccess")]
public class DetailsModel : PageModel
{
    private readonly UserManager<Employee> _userManager;

    public DetailsModel(UserManager<Employee> userManager)
    {
        _userManager = userManager;
    }

    public Employee EmployeeDetails { get; set; }

    public async Task<IActionResult> OnGetAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        EmployeeDetails = user;
        if (EmployeeDetails == null)
        {
            return NotFound();
        }

        return Page();
    }
}

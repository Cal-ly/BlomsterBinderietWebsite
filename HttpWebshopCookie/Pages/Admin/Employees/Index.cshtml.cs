namespace HttpWebshopCookie.Pages.Admin.Employees;

[Authorize(Policy = "managerAccess")]
public class IndexModel : PageModel
{
    private readonly UserManager<Employee> _userManager;

    public IndexModel(UserManager<Employee> userManager)
    {
        _userManager = userManager;
    }

    public List<Employee> Employees { get; set; } = new();

    public async Task OnGetAsync()
    {
        Employees = new List<Employee>(await _userManager.GetUsersInRoleAsync("manager"));
        Employees.AddRange(await _userManager.GetUsersInRoleAsync("staff"));
        Employees.AddRange(await _userManager.GetUsersInRoleAsync("assistant"));
        Employees.AddRange(await _userManager.GetUsersInRoleAsync("companyrep"));
    }
}

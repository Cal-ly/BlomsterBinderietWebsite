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
        Employees =
        [
            .. await _userManager.GetUsersInRoleAsync("manager"),
            .. await _userManager.GetUsersInRoleAsync("staff"),
            .. await _userManager.GetUsersInRoleAsync("assistant"),
            .. await _userManager.GetUsersInRoleAsync("companyrep"),
        ];

        if(User.IsInRole("admin"))
        {
            Employees.AddRange(await _userManager.GetUsersInRoleAsync("admin"));
        }
    }
}

namespace HttpWebshopCookie.Pages.Admin.Employees;


[Authorize(Policy = "managerAccess")]
public class CreateModel : PageModel
{
    private readonly UserManager<Employee> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<ApplicationUser> _applicationUserManager;

    public CreateModel(UserManager<Employee> userManager, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> applicationUserManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _applicationUserManager = applicationUserManager;
    }

    [BindProperty]
    public RegisterModel Input { get; set; } = null!;

    public List<string> AvailableRoles { get; set; } = new();

    public async Task OnGetAsync()
    {
        var user = await _applicationUserManager.GetUserAsync(User);
        var userRoles = await _applicationUserManager.GetRolesAsync(user!);

        if (userRoles.Contains("admin"))
        {
            AvailableRoles = new List<string> { "admin", "manager", "staff", "assistant" };
        }
        else if (userRoles.Contains("manager"))
        {
            AvailableRoles = new List<string> { "manager", "staff", "assistant" };
        }
        else
        {
            AvailableRoles = new List<string>();
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (ModelState.IsValid)
        {
            var user = new Employee { UserName = Input.Email, Email = Input.Email, FirstName = Input.FirstName, LastName = Input.LastName, JobTitle = Input.JobTitle, Salary = Input.Salary, EnrollmentDate = Input.EnrollmentDate };
            if (string.IsNullOrEmpty(Input.Password))
            {
                ModelState.AddModelError(string.Empty, "Password cannot be null or empty.");
                return Page();
            }
            var result = await _userManager.CreateAsync(user, Input.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, Input.Role!);
                return RedirectToPage("Index");
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
        return Page();
    }

    public class RegisterModel
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Role { get; set; }
        public string? JobTitle { get; set; }
        public decimal Salary { get; set; }
        public DateTime? EnrollmentDate { get; set; }
    }
}
namespace HttpWebshopCookie.Pages.Admin.Employees;

[Authorize(Policy = "managerAccess")]
public class EditModel : PageModel
{
    private readonly UserManager<Employee> _userManager;
    private readonly UserManager<ApplicationUser> _applicationUserManager;

    public EditModel(UserManager<Employee> userManager, UserManager<ApplicationUser> applicationUserManager)
    {
        _userManager = userManager;
        _applicationUserManager = applicationUserManager;
    }

    [BindProperty]
    public EditInputModel Input { get; set; } = null!;

    public List<string> AvailableRoles { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        Input = new EditInputModel
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            JobTitle = user.JobTitle,
            Salary = user.Salary,
            EnrollmentDate = user.EnrollmentDate,
            TerminationDate = user.TerminationDate
        };

        var currentUser = await _applicationUserManager.GetUserAsync(User);
        var currentUserRoles = await _applicationUserManager.GetRolesAsync(currentUser!);

        if (currentUserRoles.Contains("admin"))
        {
            AvailableRoles = new List<string> { "admin", "manager", "staff", "assistant" };
        }
        else if (currentUserRoles.Contains("manager"))
        {
            AvailableRoles = new List<string> { "manager", "staff", "assistant" };
        }
        else
        {
            AvailableRoles = new List<string>();
        }

        Input.Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault();

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = await _userManager.FindByIdAsync(Input.Id!);
        if (user == null)
        {
            return NotFound();
        }

        user.FirstName = Input.FirstName;
        user.LastName = Input.LastName;
        user.Email = Input.Email;
        user.UserName = Input.Email;
        user.JobTitle = Input.JobTitle;
        user.Salary = Input.Salary;
        user.EnrollmentDate = Input.EnrollmentDate;
        user.TerminationDate = Input.TerminationDate ?? null;

        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded)
        {
            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, Input.Role!);

            return RedirectToPage("./Index");
        }
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
        return Page();
    }

    public class EditInputModel
    {
        public string? Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
        public string? JobTitle { get; set; }
        public decimal? Salary { get; set; }
        public DateTime? EnrollmentDate { get; set; }
        public DateTime? TerminationDate { get; set; }
    }
}
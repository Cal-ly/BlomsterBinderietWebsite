namespace HttpWebshopCookie.Pages.Admin.Employees;

[Authorize(Policy = "managerAccess")]
public class EditModel : PageModel
{
    private readonly UserManager<Employee> _userManager;

    public EditModel(UserManager<Employee> userManager)
    {
        _userManager = userManager;
    }

    [BindProperty]
    public EditInputModel Input { get; set; }

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
            Salary = user.Salary ?? 0,
            EnrollmentDate = user.EnrollmentDate ?? DateTime.UtcNow,
            TerminationDate = user.TerminationDate
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = await _userManager.FindByIdAsync(Input.Id);
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
        user.TerminationDate = Input.TerminationDate;

        var result = await _userManager.UpdateAsync(user);
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

    public class EditInputModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string JobTitle { get; set; }
        public decimal Salary { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public DateTime? TerminationDate { get; set; }
    }
}
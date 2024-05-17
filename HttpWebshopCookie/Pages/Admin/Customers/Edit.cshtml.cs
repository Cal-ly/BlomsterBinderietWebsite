namespace HttpWebshopCookie.Pages.Admin.Customers;

[Authorize(Policy = "managerAccess")]
public class EditModel : PageModel
{
    private readonly UserManager<Customer> _userManager;
    private readonly ApplicationDbContext _context;

    public EditModel(UserManager<Customer> userManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    [BindProperty]
    public EditInputModel Input { get; set; } = new EditInputModel();

    public List<string> AvailableRoles { get; set; } = new List<string>();
    public List<Company> Companies { get; set; } = new List<Company>();

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
            BirthDate = user.BirthDate,
            CompanyId = user.CompanyId
        };

        AvailableRoles = new List<string> { "customer", "companyrep" };
        Companies = await _context.Companies.ToListAsync();
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
        user.BirthDate = Input.BirthDate;
        user.CompanyId = Input.CompanyId;

        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded)
        {
            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, Input.Role!);

            return RedirectToPage("Index");
        }
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        Companies = await _context.Companies.ToListAsync();
        return Page();
    }

    public class EditInputModel
    {
        public string? Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? CompanyId { get; set; }
    }
}

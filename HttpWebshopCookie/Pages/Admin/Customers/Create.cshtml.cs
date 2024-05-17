namespace HttpWebshopCookie.Pages.Admin.Customers;

[Authorize(Policy = "managerAccess")]
public class CreateModel : PageModel
{
    private readonly UserManager<Customer> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ApplicationDbContext _context;

    public CreateModel(UserManager<Customer> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
    }

    [BindProperty]
    public RegisterModel Input { get; set; } = new RegisterModel();

    public List<string> AvailableRoles { get; set; } = new List<string>();
    public List<Company> Companies { get; set; } = new List<Company>();

    public async Task OnGetAsync()
    {
        AvailableRoles = new List<string> { "customer", "companyrep" };
        Companies = await _context.Companies.ToListAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (ModelState.IsValid)
        {
            var user = new Customer
            {
                UserName = Input.Email,
                Email = Input.Email,
                FirstName = Input.FirstName,
                LastName = Input.LastName,
                BirthDate = Input.BirthDate,
                CompanyId = Input.CompanyId
            };

            var result = await _userManager.CreateAsync(user, Input.Password!);
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

        // Reload companies in case of error
        Companies = await _context.Companies.ToListAsync();
        return Page();
    }

    public class RegisterModel
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Role { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? CompanyId { get; set; }
    }
}

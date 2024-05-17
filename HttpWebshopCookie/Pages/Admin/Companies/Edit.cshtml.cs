namespace HttpWebshopCookie.Pages.Admin.Companies;

[Authorize(Policy = "managerAccess")]
public class EditModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public EditModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public CompanyInputModel Input { get; set; } = new CompanyInputModel();

    public List<Customer> CompanyReps { get; set; } = new List<Customer>();

    public async Task<IActionResult> OnGetAsync(string id)
    {
        var company = await _context.Companies.Include(c => c.Address).Include(c => c.Representatives).FirstOrDefaultAsync(c => c.Id == id);
        if (company == null)
        {
            return NotFound();
        }

        Input = new CompanyInputModel
        {
            Id = company.Id,
            Name = company.Name,
            CVR = company.CVR,
            PhoneNumber = company.PhoneNumber,
            Street = company.Address?.Street,
            PostalCode = company.Address?.PostalCode,
            City = company.Address?.City,
            Country = company.Address?.Country
        };

        CompanyReps = await _context.Customers.Where(c => c.CompanyId == id).ToListAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var company = await _context.Companies.Include(c => c.Address).FirstOrDefaultAsync(c => c.Id == Input.Id);
        if (company == null)
        {
            return NotFound();
        }

        company.Name = Input.Name;
        company.CVR = Input.CVR;
        company.PhoneNumber = Input.PhoneNumber;
        company.Address!.Street = Input.Street;
        company.Address!.PostalCode = Input.PostalCode;
        company.Address!.City = Input.City;
        company.Address!.Country = Input.Country;

        await _context.SaveChangesAsync();
        return RedirectToPage("Index");
    }

    public class CompanyInputModel
    {
        public string? Id { get; set; } = null;
        public string? Name { get; set; } = null;
        public string? CVR { get; set; } = null;
        public string? PhoneNumber { get; set; }
        public string? Street { get; set; } = string.Empty;
        public string? PostalCode { get; set; } = string.Empty;
        public string? City { get; set; } = string.Empty;
        public string? Country { get; set; } 
    }
}
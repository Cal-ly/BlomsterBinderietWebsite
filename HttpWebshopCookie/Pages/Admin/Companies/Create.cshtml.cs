namespace HttpWebshopCookie.Pages.Admin.Companies;

[Authorize(Policy = "managerAccess")]
public class CreateModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public CreateModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public CompanyInputModel Input { get; set; } = new CompanyInputModel();

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (ModelState.IsValid)
        {
            var address = new Address
            {
                Street = Input.Street,
                PostalCode = Input.PostalCode,
                City = Input.City,
                Country = Input.Country
            };

            _context.Addresses.Add(address);
            await _context.SaveChangesAsync();

            var company = new Company
            {
                Name = Input.Name,
                CVR = Input.CVR,
                PhoneNumber = Input.PhoneNumber,
                AddressId = address.Id
            };

            _context.Companies.Add(company);
            await _context.SaveChangesAsync();

            return RedirectToPage("Index");
        }
        return Page();
    }

    public class CompanyInputModel
    {
        public string? Name { get; set; }
        public string? CVR { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Street { get; set; }
        public string? PostalCode { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
    }
}
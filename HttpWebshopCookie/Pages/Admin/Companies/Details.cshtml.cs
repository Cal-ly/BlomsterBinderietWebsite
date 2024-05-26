namespace HttpWebshopCookie.Pages.Admin.Companies;

[Authorize(Policy = "managerAccess")]
public class DetailsModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public DetailsModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public Company CompanyDetails { get; set; } = new Company();

    public async Task<IActionResult> OnGetAsync(string id)
    {
        Company? companyDetails = await _context.Companies.Include(c => c.Address).Include(c => c.Representatives).FirstOrDefaultAsync(c => c.Id == id);
        if (companyDetails == null)
        {
            return NotFound();
        }
        CompanyDetails = companyDetails;

        return Page();
    }
}
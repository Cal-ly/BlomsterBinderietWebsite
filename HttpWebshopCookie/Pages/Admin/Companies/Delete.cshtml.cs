namespace HttpWebshopCookie.Pages.Admin.Companies;

[Authorize(Policy = "managerAccess")]
public class DeleteModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public DeleteModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public Company CompanyToDelete { get; set; } = new Company();
    public Address AddressToDelete { get; set; } = new Address();

    public async Task<IActionResult> OnGetAsync(string id)
    {
        Company? companyToDelete = await _context.Companies.Include(c => c.Address).FirstOrDefaultAsync(c => c.Id == id);
        if (companyToDelete == null)
        {
            return NotFound();
        }
        CompanyToDelete = companyToDelete;
        if (companyToDelete.Address == null)
        {
            return NotFound();
        }
        AddressToDelete = companyToDelete.Address;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (CompanyToDelete == null || AddressToDelete == null)
        {
            return NotFound();
        }

        _context.Addresses.Remove(AddressToDelete);
        _context.Companies.Remove(CompanyToDelete);
        await _context.SaveChangesAsync();
        return RedirectToPage("Index");
    }
}
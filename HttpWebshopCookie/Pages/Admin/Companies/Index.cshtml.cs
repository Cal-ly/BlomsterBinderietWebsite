using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HttpWebshopCookie.Pages.Admin.Companies;

[Authorize(Policy = "managerAccess")]
public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public List<Company> Companies { get; set; } = new List<Company>();

    public async Task OnGetAsync()
    {
        Companies = await _context.Companies.Include(c => c.Address).Include(c => c.Representatives).ToListAsync();
    }
}
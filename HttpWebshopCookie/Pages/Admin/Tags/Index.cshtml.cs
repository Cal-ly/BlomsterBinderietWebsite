using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HttpWebshopCookie.Pages.Admin.Tags
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Tag> Tags { get; set; } = default!;

        public async Task OnGetAsync()
        {
            Tags = await _context.Tags.ToListAsync();
        }
    }
}

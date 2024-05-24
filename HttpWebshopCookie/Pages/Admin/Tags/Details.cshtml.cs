using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using HttpWebshopCookie.Data;
using HttpWebshopCookie.Models;

namespace HttpWebshopCookie.Pages.Admin.Tags
{
    public class DetailsModel : PageModel
    {
        private readonly HttpWebshopCookie.Data.ApplicationDbContext _context;

        public DetailsModel(HttpWebshopCookie.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public Tag Tag { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tag = await _context.Tags.FirstOrDefaultAsync(m => m.Id == id);
            if (tag == null)
            {
                return NotFound();
            }
            else
            {
                Tag = tag;
            }
            return Page();
        }
    }
}

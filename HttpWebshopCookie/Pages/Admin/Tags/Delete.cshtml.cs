namespace HttpWebshopCookie.Pages.Admin.Tags
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
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

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tag = await _context.Tags.FindAsync(id);
            if (tag != null)
            {
                Tag = tag;
                _context.Tags.Remove(Tag);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}

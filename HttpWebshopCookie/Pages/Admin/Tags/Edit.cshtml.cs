namespace HttpWebshopCookie.Pages.Admin.Tags
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Tag Tag { get; set; } = default!;

        [BindProperty]
        public string SelectedOccasion { get; set; } = string.Empty;

        [BindProperty]
        public string NewOccasion { get; set; } = string.Empty;

        public List<string> Occasions { get; set; } = new List<string>();

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Tag = await _context.Tags.FirstOrDefaultAsync(m => m.Id == id) ?? null!;

            if (Tag == null)
            {
                return NotFound();
            }

            Occasions = await _context.Tags
                                      .Select(t => t.Occasion!)
                                      .Distinct()
                                      .ToListAsync();
            SelectedOccasion = Tag.Occasion!;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Tag.Occasion = !string.IsNullOrEmpty(NewOccasion) ? NewOccasion : SelectedOccasion;

            _context.Attach(Tag).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TagExists(Tag.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool TagExists(string id)
        {
            return _context.Tags.Any(e => e.Id == id);
        }
    }
}

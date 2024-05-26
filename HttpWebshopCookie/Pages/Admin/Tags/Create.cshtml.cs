namespace HttpWebshopCookie.Pages.Admin.Tags
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
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

        public async Task<IActionResult> OnGetAsync()
        {
            Occasions = await _context.Tags
                                      .Select(t => t.Occasion!)
                                      .Distinct()
                                      .ToListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Tag.Occasion = !string.IsNullOrEmpty(NewOccasion) ? NewOccasion : SelectedOccasion;

            _context.Tags.Add(Tag);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}

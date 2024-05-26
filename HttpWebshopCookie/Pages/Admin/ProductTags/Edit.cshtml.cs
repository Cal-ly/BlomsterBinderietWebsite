namespace HttpWebshopCookie.Pages.Admin.ProductTags
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Product Product { get; set; } = default!;

        [BindProperty]
        public List<Tag> ProductTags { get; set; } = new List<Tag>();

        [BindProperty]
        public List<Tag> AvailableTags { get; set; } = new List<Tag>();

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Product = await _context.Products
                .Include(p => p.ProductTags)
                .ThenInclude(pt => pt.Tag)
                .FirstOrDefaultAsync(m => m.Id == id) ?? null!;

            if (Product == null)
            {
                return NotFound();
            }

            ProductTags = Product.ProductTags.Select(pt => pt.Tag!).ToList();
            AvailableTags = await _context.Tags.Where(t => !ProductTags.Select(pt => pt.Id).Contains(t.Id)).ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAddTagAsync(string id, string tagId)
        {
            if (id == null || tagId == null)
            {
                return NotFound();
            }

            var productTag = new IXProductTag
            {
                ProductId = id,
                TagId = tagId
            };

            _context.ProductTags.Add(productTag);
            await _context.SaveChangesAsync();

            return RedirectToPage(new { id });
        }

        public async Task<IActionResult> OnPostRemoveTagAsync(string id, string tagId)
        {
            if (id == null || tagId == null)
            {
                return NotFound();
            }

            var productTag = await _context.ProductTags.FirstOrDefaultAsync(pt => pt.ProductId == id && pt.TagId == tagId);

            if (productTag != null)
            {
                _context.ProductTags.Remove(productTag);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage(new { id });
        }
    }
}

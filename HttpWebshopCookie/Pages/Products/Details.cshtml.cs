namespace HttpWebshopCookie.Pages.Products
{
    public class DetailsModel(ApplicationDbContext context) : PageModel
    {
        public Product Product { get; set; } = default!;
        public List<string> ProductTagStrings { get; set; } = [];

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await context.Products.FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            else
            {
                Product = product;
                var productTags = await context.ProductTags
                    .Include(pt => pt.Tag)
                    .Where(pt => pt.ProductId == id)
                    .Select(pt => pt.Tag)
                    .ToListAsync();

                foreach (var tag in productTags)
                {
                    if (tag?.Occasion != null && tag.Category != null && tag.SubCategory != null)
                    {
                        if (!ProductTagStrings.Contains(tag.Occasion))
                        {
                            ProductTagStrings.Add(tag.Occasion);
                        }
                        if (!ProductTagStrings.Contains(tag.Category))
                        {
                            ProductTagStrings.Add(tag.Category);
                        }
                        if (!ProductTagStrings.Contains(tag.SubCategory))
                        {
                            ProductTagStrings.Add(tag.SubCategory);
                        }
                    }
                }
            }
            return Page();
        }
    }
}

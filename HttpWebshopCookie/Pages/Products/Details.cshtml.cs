namespace HttpWebshopCookie.Pages.Products;

public class DetailsModel(ApplicationDbContext context) : PageModel
{
    public Product Product { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(string id)
    {
        if (id == null)
        {
            return NotFound();
        }

        Product? product = await context.Products.AsNoTracking()
                                .FirstOrDefaultAsync(m => m.Id == id);
        if (product == null)
        {
            return NotFound();
        }
        else
        {
            Product = product;
        }
        return Page();
    }
}

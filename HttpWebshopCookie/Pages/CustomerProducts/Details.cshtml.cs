namespace HttpWebshopCookie.Pages.CustomerProducts;

public class DetailsModel(ApplicationDbContext context) : PageModel
{
    public Product Product { get; set; } = default!;

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
        }
        return Page();
    }
}

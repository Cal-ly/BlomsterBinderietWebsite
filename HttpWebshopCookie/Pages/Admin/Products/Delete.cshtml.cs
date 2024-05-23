namespace HttpWebshopCookie.Pages.Products;

[Authorize(Policy = "staffAccess")]
public class DeleteModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public DeleteModel(ApplicationDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    [BindProperty]
    public Product Product { get; set; } = default!;

    [BindProperty]
    public bool PerformTrueDelete { get; set; } = false;

    public async Task<IActionResult> OnGetAsync(string id)
    {
        if (id == null)
        {
            return NotFound();
        }

        Product? product = await _context.Products
                                .AsNoTracking()
                                .FirstOrDefaultAsync(m => m.Id == id);

        if (product == null)
        {
            return NotFound();
        }
        Product = product;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var productToDelete = await _context.Products.FindAsync(id);
        if (productToDelete != null)
        {
            // True delete functionality
            if ((User.IsInRole("manager") || User.IsInRole("admin") || User.IsInRole("staff")) && PerformTrueDelete)
            {
                if (!string.IsNullOrWhiteSpace(productToDelete.ImageUrl) && !productToDelete.ImageUrl.Contains("default.jpg"))
                {
                    var imagePath = Path.Combine(_environment.WebRootPath, productToDelete.ImageUrl);
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }
                _context.Products.Remove(productToDelete);
                await _context.SaveChangesAsync();
            }
            else
            {
                // Soft delete functionality
                productToDelete.IsDeleted = !productToDelete.IsDeleted;
                await _context.SaveChangesAsync();
            }

            _context.Products.Remove(productToDelete);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage("./Index");
    }
}
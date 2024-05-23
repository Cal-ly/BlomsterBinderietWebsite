namespace HttpWebshopCookie.Pages.Products;


[Authorize(Policy = "staffAccess")]
public class EditModel(ApplicationDbContext context, IWebHostEnvironment environment) : PageModel
{
    [BindProperty]
    public Product Product { get; set; } = null!;

    [BindProperty]
    public IFormFile? UploadFile { get; set; }

    public async Task<IActionResult> OnGetAsync(string id)
    {
        if (id == null)
        {
            return NotFound();
        }

        Product? getProduct = await context.Products.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);

        if (getProduct == null)
        {
            return NotFound();
        }

        Product = getProduct;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string id)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var productToUpdate = await context.Products.FirstOrDefaultAsync(p => p.Id == id);

        if (productToUpdate == null)
        {
            return NotFound();
        }

        if (UploadFile != null && IsValidImageFile(UploadFile))
        {
            // Handle image replacement, including deletion of the old image
            if (!string.IsNullOrWhiteSpace(productToUpdate.ImageUrl) && !productToUpdate.ImageUrl.Contains("default.jpg"))
            {
                var oldImagePath = Path.Combine(environment.WebRootPath, productToUpdate.ImageUrl);
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }

            // Save new image
            var uniqueFileName = $"{Guid.NewGuid()}-{UploadFile.FileName}";
            var filePath = Path.Combine(environment.WebRootPath, "images", "products", uniqueFileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await UploadFile.CopyToAsync(stream);
            }

            productToUpdate.ImageUrl = Path.Combine("images", "products", uniqueFileName);
            productToUpdate.ImageUrl = productToUpdate.ImageUrl.Replace("\\", "/");
            productToUpdate.UpdatedAt = DateTime.Now;

        }

        if (await TryUpdateModelAsync<Product>(productToUpdate, "Product", p => p.Name, p => p.Description, p => p.Price, p => p.IsDeleted, p => p.ImageUrl))
        {
            await context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }

        return !ProductExists(productToUpdate.Id) ? NotFound() : Page();
    }

    private bool IsValidImageFile(IFormFile file)
    {
        var allowedExtensions = new HashSet<string> { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
        var allowedMimeTypes = new HashSet<string> { "image/jpeg", "image/png", "image/gif", "image/bmp" };
        var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

        return allowedExtensions.Contains(fileExtension) && allowedMimeTypes.Contains(file.ContentType.ToLowerInvariant());
    }

    private bool ProductExists(string id)
    {
        return context.Products.Any(e => e.Id == id);
    }
}

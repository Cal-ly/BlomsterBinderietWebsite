using Microsoft.EntityFrameworkCore;

namespace HttpWebshopCookie.Pages.Products;

public class CreateModel(ApplicationDbContext context, IWebHostEnvironment environment) : PageModel
{
    [BindProperty]
    public Product Product { get; set; } = new Product();

    [BindProperty]
    public IFormFile? UploadFile { get; set; }

    public IActionResult OnGet()
    {
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        const long maxFileSize = 10 * 1024 * 1024; // 10 * 2^10 * 2^10 = 10 MB

        if (UploadFile != null)
        {
            if (UploadFile.Length > maxFileSize)
            {
                ModelState.AddModelError("UploadFile", "File size must not exceed 10 MB.");
                return Page();
            }

            if (!IsValidImageFile(UploadFile))
            {
                ModelState.AddModelError("UploadFile", "Invalid file type. Only images are allowed.");
                return Page();
            }

            var uniqueFileName = $"{Guid.NewGuid()}-{UploadFile.FileName}";
            var filePath = Path.Combine(environment.WebRootPath, "images", "products", uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await UploadFile.CopyToAsync(stream);
            }

            Product.ImageUrl = $"/images/products/{uniqueFileName}";
        }
        else
        {
            //TODO implement default behavior?
            Product.ImageUrl = null;
        }

        context.Products.Add(Product);
        await context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }

    private static bool IsValidImageFile(IFormFile file)
    {
        var allowedExtensions = new HashSet<string> { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
        var allowedMimeTypes = new HashSet<string> { "image/jpeg", "image/png", "image/gif", "image/bmp" };
        var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

        return allowedExtensions.Contains(fileExtension) && allowedMimeTypes.Contains(file.ContentType.ToLowerInvariant());
    }
}
using Microsoft.AspNetCore.Mvc;

namespace HttpWebshopCookie.Pages.Products
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly BasketService _basketService;

        public IndexModel(ApplicationDbContext context, BasketService basketService)
        {
            _context = context;
            _basketService = basketService;
        }

        public List<Product> ProductList { get; set; } = default!;

        public async Task OnGetAsync()
        {
            ProductList = await _context.Products.OrderBy(p => p.Name).ToListAsync();
        }

        public IActionResult OnPostAddToBasket(string id)
        {
            if (!_context.Products.Any(p => p.Id == id))
            {
                return NotFound();
            }

            _basketService.AddToBasket(id);

            return RedirectToPage("./Index");
        }
    }
}

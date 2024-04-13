namespace HttpWebshopCookie.Pages.Products;

public class IndexModel(ApplicationDbContext context, BasketService basketService) : PageModel
{
    [BindProperty]
    public List<Product> ProductList { get; set; } = default!;
    [BindProperty]
    public Dictionary<string, int> ProductQuantities { get; set; } = [];

    public async Task OnGetAsync()
    {
        ProductList = await context.Products.OrderBy(p => p.Name).ToListAsync();
        var basket = basketService.GetOrCreateBasket();
        ProductQuantities = await basketService.GetAllQuantitiesInBasket();
    }

    public async Task<IActionResult> OnPostAddToBasket(string id)
    {
        if (!await context.Products.AnyAsync(p => p.Id == id))
        {
            return NotFound();
        }

        await basketService.AddToBasket(id);
        ProductQuantities[id] = await basketService.GetQuantityInBasket(id);
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostRemoveFromBasket(string id)
    {
        if (!await context.Products.AnyAsync(p => p.Id == id))
        {
            return NotFound();
        }

        await basketService.RemoveFromBasket(id);
        ProductQuantities[id] = await basketService.GetQuantityInBasket(id);

        return RedirectToPage();
    }
}

namespace HttpWebshopCookie.Pages.Products;

public class IndexModel(ApplicationDbContext context, BasketService basketService) : PageModel
{
    [BindProperty]
    public List<Product> ProductList { get; set; } = default!;

    public Dictionary<string, int> ProductQuantities { get; set; } = new Dictionary<string, int>();

    public async Task OnGetAsync()
    {
        ProductList = await context.Products.OrderBy(p => p.Name).ToListAsync();
        var basketQuantities = await basketService.GetAllQuantitiesInBasket();
        ProductQuantities = ProductList.ToDictionary(p => p.Id, p => basketQuantities.ContainsKey(p.Id) ? basketQuantities[p.Id] : 0);
    }

    public async Task<IActionResult> OnPostAddToBasket(string id)
    {
        if (!await context.Products.AnyAsync(p => p.Id == id))
        {
            return NotFound();
        }

        await basketService.AddToBasket(id);
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostRemoveFromBasket(string id)
    {
        if (!await context.Products.AnyAsync(p => p.Id == id))
        {
            return NotFound();
        }
        await basketService.RemoveFromBasket(id);

        return RedirectToPage();
    }
}
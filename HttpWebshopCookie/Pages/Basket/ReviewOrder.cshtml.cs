namespace HttpWebshopCookie.Pages.Basket;

public class ReviewOrderModel(BasketService basketService) : PageModel
{
    public Models.Basket Basket { get; set; } = default!;

    [BindProperty]
    public Dictionary<string, int> Quantities { get; set; } = new Dictionary<string, int>();

    public void OnGet()
    {
        Basket = basketService.GetOrCreateBasket();
    }

    public IActionResult OnPostUpdateBasket()
    {
        foreach (var entry in Quantities)
        {
            basketService.UpdateBasketItemQuantity(entry.Key, entry.Value).Wait();
        }

        return RedirectToPage();
    }

    public IActionResult OnPostProceedToCheckout()
    {
        return RedirectToPage("ReviewInfo");
    }
    public IActionResult OnPostContinueShopping()
    {
        return RedirectToPage("/Products/Index");
    }
}
namespace HttpWebshopCookie.Pages.Basket;

public class ReviewOrderModel(BasketService basketService) : PageModel
{
    public Models.Basket Basket { get; set; } = default!;

    public void OnGet()
    {
        Basket = basketService.GetOrCreateBasket();
    }

    public IActionResult OnPostUpdateBasket(string productId, int quantity)
    {
        basketService.UpdateBasketItemQuantity(productId, quantity).Wait();
        return RedirectToPage();
    }

    public IActionResult OnPostProceedToCheckout()
    {
        return RedirectToPage("ReviewInfo");
    }
}
using HttpWebshopCookie.Services;

namespace HttpWebshopCookie.ViewComponents;

public class BasketViewComponent : ViewComponent
{
    private readonly BasketService _basketService;

    public BasketViewComponent(BasketService basketService)
    {
        _basketService = basketService;
    }
    public IViewComponentResult Invoke()
    {
        var basket = _basketService.GetOrCreateBasket();
        return View(basket);
    }
}

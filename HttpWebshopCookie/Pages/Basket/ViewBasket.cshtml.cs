using HttpWebshopCookie.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HttpWebshopCookie.Pages.Basket;

public class ViewBasketModel(BasketService basketService) : PageModel
{
    public Models.Basket Basket { get; set; } = default!;

    public void OnGet()
    {
        Basket = basketService.GetOrCreateBasket();
    }
}
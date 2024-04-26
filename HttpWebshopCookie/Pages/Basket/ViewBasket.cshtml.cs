using HttpWebshopCookie.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HttpWebshopCookie.Pages.Basket;

public class ViewBasketModel(BasketService basketService) : PageModel
{
    public Models.Basket Basket { get; set; } = default!;
    public UserWrapper? UserWrapper { get; set; }

    public void OnGet()
    {
        Basket = basketService.GetOrCreateBasket();
    }
}
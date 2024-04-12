using HttpWebshopCookie.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace HttpWebshopCookie.Pages.Basket
{
    public class ViewBasketModel : PageModel
    {

        private readonly ApplicationDbContext _context;
        private readonly BasketService _basketService;

        public Models.Basket Basket { get; set; } = default!;

        public ViewBasketModel(ApplicationDbContext context, BasketService basketService)
        {
            _context = context;
            _basketService = basketService;
        }

        public void OnGet()
        {
            Basket = _basketService.GetOrCreateBasket();
        }
    }
}

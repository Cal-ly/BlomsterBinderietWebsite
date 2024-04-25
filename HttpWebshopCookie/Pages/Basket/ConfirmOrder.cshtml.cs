using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HttpWebshopCookie.Models;
using Microsoft.AspNetCore.Identity;

namespace HttpWebshopCookie.Pages.Basket;

public class ConfirmOrderModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly BasketService _basketService;
    private readonly UserManager<ApplicationUser> _userManager;

    public Models.Basket Basket { get; private set; }
    public UserWrapper UserWrapper { get; private set; }

    public ConfirmOrderModel(ApplicationDbContext context, BasketService basketService, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _basketService = basketService;
        _userManager = userManager;
    }

    public async Task<IActionResult> OnGetAsync(UserWrapper userWrapper)
    {
        UserWrapper = userWrapper ?? throw new ArgumentNullException(nameof(userWrapper));
        Basket = _basketService.GetOrCreateBasket();

        if (Basket.Items.Count == 0)
        {
            TempData["Error"] = "Your basket is empty.";
            return RedirectToPage("/Basket/ViewBasket");
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid || UserWrapper == null)
        {
            return Page(); // Return with error message
        }

        // Assume PlaceOrder completes the order process
        var orderResult = _basketService.PlaceOrder(UserWrapper);

        if (orderResult != null)
        {
            TempData["Success"] = "Your order has been placed successfully!";
            return RedirectToPage("/Basket/OrderSuccess", new { orderId = orderResult.Id });
        }
        else
        {
            ModelState.AddModelError("", "Failed to place order. Please try again.");
            return Page();
        }
    }
}

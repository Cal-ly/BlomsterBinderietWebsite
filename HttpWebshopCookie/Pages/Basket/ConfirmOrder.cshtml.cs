using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using HttpWebshopCookie.Models;
using Microsoft.AspNetCore.Identity;

namespace HttpWebshopCookie.Pages.Basket;

public class ConfirmOrderModel(BasketService basketService) : PageModel
{
    public Models.Basket? Basket { get; private set; }
    public UserWrapper? UserWrapper { get; private set; }

    public IActionResult OnGet(UserWrapper userWrapper)
    {
        UserWrapper = userWrapper ?? throw new ArgumentNullException(nameof(userWrapper));
        Basket = basketService.GetOrCreateBasket();

        if (Basket.Items.Count == 0)
        {
            TempData["Error"] = "Your basket is empty.";
            return RedirectToPage("/Basket/ViewBasket");
        }

        return Page();
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid || UserWrapper == null)
        {
            return Page(); // Return with error message
        }

        // Assume PlaceOrder completes the order process
        var orderResult = basketService.PlaceOrder(UserWrapper);

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

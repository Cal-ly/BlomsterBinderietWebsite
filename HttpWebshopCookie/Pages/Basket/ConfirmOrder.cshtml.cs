using System.Text.Json;

namespace HttpWebshopCookie.Pages.Basket;

public class ConfirmOrderModel(BasketService basketService) : PageModel
{
    public Models.Basket Basket { get; set; } = default!;
    public UserWrapper? UserWrapper { get; private set; }
    [BindProperty]
    public UserInfoModel UserInfo { get; set; } = new UserInfoModel();
    public class UserInfoModel
    {
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        public string Address { get; set; } = default!;
    }


    public IActionResult OnGet()
    {
        var userWrapperJson = HttpContext.Session.GetString("UserWrapper");

        if (!string.IsNullOrEmpty(userWrapperJson))
        {
            UserWrapper = JsonSerializer.Deserialize<UserWrapper>(userWrapperJson);
        }

        if (UserWrapper == null)
        {
            TempData["Error"] = "Failed to retrieve user information. Please try again.";
            return RedirectToPage("/Basket/ViewBasket");
        }

        Basket = basketService.GetOrCreateBasket();
        if (Basket.Items.Count == 0)
        {
            TempData["Error"] = "Your basket is empty.";
            return RedirectToPage("/Basket/ViewBasket");
        }

        UserInfo = new UserInfoModel
        {
            FirstName = UserWrapper.FirstName,
            LastName = UserWrapper.LastName,
            Email = UserWrapper.Email,
            PhoneNumber = UserWrapper.PhoneNumber,
            Address = UserWrapper.Address?.ToString() ?? string.Empty
        };

        return Page();
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid || UserWrapper == null)
        {
            ModelState.AddModelError("", "Failed to place order. Please try again.");
            return RedirectToPage();
        }

        var orderResult = basketService.PlaceOrder(UserWrapper);

        if (orderResult != null)
        {
            TempData["Success"] = "Your order has been placed successfully!";
            return RedirectToPage("/Basket/OrderSuccess", new { orderId = orderResult.Id });
        }
        else
        {
            ModelState.AddModelError("", "Failed to place order. Please try again.");
            return RedirectToPage();
        }
    }
}
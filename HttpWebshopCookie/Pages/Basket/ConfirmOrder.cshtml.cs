using Newtonsoft.Json;

namespace HttpWebshopCookie.Pages.Basket;

public class ConfirmOrderModel(BasketService basketService) : PageModel
{
    [TempData]
    public string UserWrapJson { get; set; } = null!;
    [BindProperty]
    public UserWrapper? UserWrapper { get; set; }
    [BindProperty]
    public UserInfoModel UserInfo { get; set; } = new UserInfoModel();

    public Models.Basket Basket { get; set; } = default!;

    public IActionResult OnGet(string userWrapJson)
    {
        if (UserWrapper == null)
        {
            UserWrapper = JsonConvert.DeserializeObject<UserWrapper>(userWrapJson);
        }

        //if (TempData["UserWrapper"] is string jsonString)
        //{
        //    UserWrapper = JsonConvert.DeserializeObject<UserWrapper>(jsonString);
        //}

        if (UserWrapper == null)
        {
            TempData["Error"] = "Failed to place order. Please try again.";
            return RedirectToPage("ViewBasket");
        }

        Basket = basketService.GetOrCreateBasket();
        if (Basket.Items.Count == 0)
        {
            TempData["Error"] = "Your basket is empty.";
            return RedirectToPage("ViewBasket");
        }

        UserInfo = new UserInfoModel
        {
            FirstName = UserWrapper.FirstName,
            LastName = UserWrapper.LastName,
            Email = UserWrapper.Email,
            PhoneNumber = UserWrapper.PhoneNumber ?? string.Empty,
            Address = UserWrapper.Address?.FullAddress() ?? string.Empty
        };

        return Page();
    }

    public IActionResult OnPost(string userWrapperId)
    {
        if (!ModelState.IsValid || UserWrapper == null)
        {
            ModelState.AddModelError("", "Failed to place order. Please try again.");
            return RedirectToPage();
        }

        if (UserWrapper.Id != userWrapperId)
        {
            TempData["Error"] = "Failed to place order. Please try again.";
            ModelState.AddModelError("", "Failed to place order. Please try again.");
            return RedirectToPage();
        }

        var orderResult = basketService.PlaceOrder(UserWrapper);

        if (orderResult != null)
        {
            TempData["Success"] = "Your order has been placed successfully!";
            return RedirectToPage("OrderSuccess", new { orderId = orderResult.Id });
        }
        else
        {
            TempData["Error"] = "Failed to place order. Please try again.";
            ModelState.AddModelError("", "Failed to place order. Please try again.");
            return RedirectToPage();
        }
    }
}
public class UserInfoModel
{
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;
    public string Address { get; set; } = default!;
}
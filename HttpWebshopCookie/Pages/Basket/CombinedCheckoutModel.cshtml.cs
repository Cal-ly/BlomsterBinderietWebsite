namespace HttpWebshopCookie.Pages.Basket;

public class CombinedCheckout(ApplicationDbContext context, BasketService basketService, UserManager<ApplicationUser> userManager) : PageModel
{
    [BindProperty]
    public Guest Guest { get; set; } = null!;
    [BindProperty]
    public Address Address { get; set; } = null!;
    [TempData]
    public string UserWrapJson { get; set; } = null!;
    public Models.Basket? Basket { get; set; }
    public UserWrapper? UserWrapper { get; private set; }

    public async Task OnGet()
    {
        Basket = basketService.GetOrCreateBasket();

        if (User!.Identity!.IsAuthenticated)
        {
            var userId = userManager.GetUserId(User);
            var user = await userManager.FindByIdAsync(userId!);

            if (user != null)
            {
                if (context.Customers.Any(c => c.Id == userId))
                {
                    var customer = await context.Customers.FindAsync(userId);
                    UserWrapper = new UserWrapper(customer!);
                }
                else if (context.Employees.Any(e => e.Id == userId))
                {
                    var employee = await context.Employees.FindAsync(userId);
                    UserWrapper = new UserWrapper(employee!);
                }
                else
                {
                    UserWrapper = new UserWrapper(user);
                }
            }
        }
        else
        {
            UserWrapper = null;
        }
    }

    public IActionResult OnPost(string userWrapperId)
    {
        if (!ModelState.IsValid)
        {
            ModelState.AddModelError("", "Failed to place order. Please try again.");
            return RedirectToPage();
        }

        if (UserWrapper == null)
        {
            if (string.IsNullOrEmpty(Guest.FirstName) || string.IsNullOrEmpty(Guest.LastName) || string.IsNullOrEmpty(Guest.Email) || string.IsNullOrEmpty(Guest.PhoneNumber) || Address == null)
            {
                ModelState.AddModelError("", "All fields are required.");
                return RedirectToPage();
            }
            // Create a new Guest and UserWrapper from the input fields
            Guest = new Guest
            {
                FirstName = Guest.FirstName,
                LastName = Guest.LastName,
                Email = Guest.Email,
                PhoneNumber = Guest.PhoneNumber,
                Address = Address
            };

            UserWrapper = new UserWrapper(Guest);
        }
        else if (UserWrapper.Id != userWrapperId)
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

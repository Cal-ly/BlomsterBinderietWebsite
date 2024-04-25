namespace HttpWebshopCookie.Pages.Basket;

public class CheckoutUserModel(ApplicationDbContext context, BasketService basketService, OrderService orderService, UserManager<ApplicationUser> userManager) : PageModel
{
    public Models.Basket Basket { get; set; } = default!;
    [BindProperty]
    public Address Address { get; set; } = default!;
    [BindProperty]
    public UserWrapper UserWrapper { get; set; } = default!;

    public void OnGet(UserWrapper userWrapper)
    {
        Basket = basketService.GetOrCreateBasket();
        if (userWrapper is not null)
        {
            UserWrapper = userWrapper;
        }
    }
    public async Task<IActionResult> OnPostCheckOutUser()
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null)
        {
            return RedirectToPage("/Account/Login", new { area = "Identity" });
        }
        Order order = basketService.Checkout(user);
        if (order is null)
        {
            return RedirectToPage("/Index");
        }
        else
        {
            return RedirectToPage("/Order/OrderConfirmation", new { id = order.Id });
        }
    }
}
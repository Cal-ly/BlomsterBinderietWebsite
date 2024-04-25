namespace HttpWebshopCookie.Pages.Basket;

public class ViewBasketModel(ApplicationDbContext context, BasketService basketService, UserManager<ApplicationUser> userManager) : PageModel
{
    public Models.Basket Basket { get; set; } = default!;
    public UserWrapper UserWrapper { get; set; } = default!;

    public Task OnGet()
    {
        Basket = basketService.GetOrCreateBasket();
        if (User.Identity is not null)
        {
            var currentUser = userManager.GetUserAsync(User);
            if (currentUser is not null)
            {
                UserWrapper = new UserWrapper(currentUser.Result);
            }
        }
        return Task.CompletedTask;
    }

}

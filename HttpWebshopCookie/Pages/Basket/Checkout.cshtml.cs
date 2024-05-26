using Newtonsoft.Json;

namespace HttpWebshopCookie.Pages.Basket;

public class CheckoutModel(ApplicationDbContext context, BasketService basketService, UserManager<ApplicationUser> userManager) : PageModel
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
    public IActionResult OnPostUser()
    {
        //if (!ModelState.IsValid)
        //{
        //    return Page();
        //}

        UserWrapJson = JsonConvert.SerializeObject(UserWrapper);
        TempData["UserWrapper"] = UserWrapJson;
        return RedirectToPage("ConfirmOrder", new { userWrapJson = UserWrapJson });
    }

    public async Task<IActionResult> OnPostGuestAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        Guest.Address = Address;
        UserWrapper = new UserWrapper(Guest);

        UserWrapJson = JsonConvert.SerializeObject(UserWrapper);
        TempData["UserWrapper"] = UserWrapJson;

        await context.SaveChangesAsync();

        return RedirectToPage("ConfirmOrder", new { userWrapJson = UserWrapJson });
    }
}
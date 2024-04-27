namespace HttpWebshopCookie.Pages.Basket;

public class CheckoutModel(ApplicationDbContext context, BasketService basketService, UserManager<ApplicationUser> userManager) : PageModel
{
    [BindProperty]
    public Guest Guest { get; set; } = new Guest();
    [BindProperty]
    public Address Address { get; set; } = new Address();
    public Models.Basket Basket { get; set; } = default!;
    public UserWrapper? UserWrapper { get; private set; }

    public async Task<IActionResult> OnGet()
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
        return Page();
    }

    public async Task<IActionResult> OnPostGuestAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // Create guest and address
        Guest.Address = Address;
        UserWrapper = new UserWrapper(Guest);
        await context.SaveChangesAsync(); // Assuming you save the guest data

        return RedirectToPage("/Basket/ConfirmOrder", new { user = UserWrapper });
    }

    public IActionResult OnPostUser()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        // Confirm order for registered user
        return RedirectToPage("/Basket/ConfirmOrder", new { user = UserWrapper });
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HttpWebshopCookie.Pages.Basket;

public class CheckoutGuestModel(ApplicationDbContext context, BasketService basketService, OrderService orderService, UserManager<ApplicationUser> userManager) : PageModel
{
    public Models.Basket Basket { get; set; } = default!;
    [BindProperty]
    public Guest Guest { get; set; } = default!;
    [BindProperty]
    public Address Address { get; set; } = default!;
    public UserWrapper UserWrapper { get; set; } = default!;

    public void OnGet()
    {
        Basket = basketService.GetOrCreateBasket();
    }
    public async Task<IActionResult> OnPostCheckOutGuest()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }
        var guest = new Guest
        {
            FirstName = Guest.FirstName,
            LastName = Guest.LastName,
            Email = Guest.Email,
            PhoneNumber = Guest.PhoneNumber
        };
        var address = new Address
        {
            Resident = Address.Resident ?? $"{Guest.FirstName} {Guest.LastName}",
            Street = Address.Street,
            City = Address.City,
            PostalCode = Address.PostalCode,
            Country = Address.Country
        };
        var user = new UserWrapper(guest);
        user.Id = Guid.NewGuid().ToString();
        Order order = basketService.Checkout(user, address);
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

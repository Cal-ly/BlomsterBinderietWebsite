using System.Security.Claims;

namespace HttpWebshopCookie.Pages.Basket;

public class ReviewInfoModel(BasketService basketService, ApplicationDbContext context) : PageModel
{
    [BindProperty]
    public ReviewInfoViewModel ReviewInfo { get; set; } = new ReviewInfoViewModel();
    public bool IsCustomer { get; set; } = false;
    public bool IsEmployee { get; set; } = false;
    public UserWrapper UserWrapper { get; set; } = null!;

    public void OnGet()
    {
        UserWrapper userWrapper = null!;
        if (User.Identity?.IsAuthenticated == true)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId != null)
            {
                var user = context.Users.Find(userId);

                if (user != null)
                {
                    if (context.Customers.Any(c => c.Id == userId))
                    {
                        var customer = context.Customers.FindAsync(userId).Result;
                        userWrapper = new UserWrapper(customer!);
                        IsCustomer = true;
                    }
                    else if (context.Employees.Any(e => e.Id == userId))
                    {
                        var employee = context.Employees.FindAsync(userId).Result;
                        userWrapper = new UserWrapper(employee!);
                        IsEmployee = true;
                    }
                    else
                    {
                        userWrapper = null!;
                    }
                }
            }

            ReviewInfo = new ReviewInfoViewModel
            {
                FirstName = userWrapper.FirstName,
                LastName = userWrapper.LastName,
                Email = userWrapper.Email,
                PhoneNumber = userWrapper.PhoneNumber,
                Resident = userWrapper?.Address?.Resident!,
                Street = userWrapper?.Address?.Street!,
                PostalCode = userWrapper?.Address?.PostalCode!,
                City = userWrapper?.Address?.City!,
                Country = userWrapper?.Address?.Country!
            };
            UserWrapper = userWrapper!;
        }
        else
        {
            ReviewInfo = new ReviewInfoViewModel();
        }
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        if (!IsCustomer && !IsEmployee)
        {
            var guestAddress = new Address
            {
                Resident = ReviewInfo.Resident,
                Street = ReviewInfo.Street,
                PostalCode = ReviewInfo.PostalCode,
                City = ReviewInfo.City,
                Country = ReviewInfo.Country
            };

            var guest = new Guest
            {
                FirstName = ReviewInfo.FirstName,
                LastName = ReviewInfo.LastName,
                Email = ReviewInfo.Email,
                PhoneNumber = ReviewInfo.PhoneNumber,
                Address = guestAddress,
                AddressId = guestAddress.Id
            };

            UserWrapper = new(guest);
        }

        var order = basketService.PlaceOrder(UserWrapper);

        return RedirectToPage("OrderSuccess", new { orderId = order.Id });
    }
}
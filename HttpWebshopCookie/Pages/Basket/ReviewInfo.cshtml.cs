namespace HttpWebshopCookie.Pages.Basket;

public class ReviewInfoModel(BasketService basketService, ApplicationDbContext context) : PageModel
{
    [BindProperty]
    public ReviewInfoViewModel ReviewInfo { get; set; } = new ReviewInfoViewModel();
    public UserWrapper? UserWrapper { get; set; }
    public Address? UserAddress { get; set; }

    public void OnGet()
    {
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
                        UserAddress = context.Addresses.Find(customer?.AddressId);
                        UserWrapper = new UserWrapper(customer!);
                    }
                    else if (context.Employees.Any(e => e.Id == userId))
                    {
                        var employee = context.Employees.FindAsync(userId).Result;
                        UserAddress = context.Addresses.Find(employee?.AddressId);
                        UserWrapper = new UserWrapper(employee!);
                    }
                    TempData["UserType"] = UserWrapper?.GetUserType();
                    TempData["UserId"] = UserWrapper?.Id;
                }
                else
                {
                    UserWrapper = null!;
                }
            }

            ReviewInfo = new ReviewInfoViewModel
            {
                FirstName = UserWrapper?.FirstName!,
                LastName = UserWrapper?.LastName!,
                Email = UserWrapper?.Email!,
                PhoneNumber = UserWrapper?.PhoneNumber!,
                Resident = UserAddress?.Resident!,
                Street = UserAddress?.Street!,
                PostalCode = UserAddress?.PostalCode!,
                City = UserAddress?.City!,
                Country = UserAddress?.Country!
            };
        }
        else
        {
            ReviewInfo = new ReviewInfoViewModel();
        }
    }

    public IActionResult OnPostSubmitOrder()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        if (User!.Identity!.IsAuthenticated)
        {
            string? userType = (string?)TempData["UserType"];
            Guid? userGuid = (Guid?)TempData["UserId"];
            string? userId = userGuid.ToString();
            if (userId != null && userType != null)
            {
                switch (userType)
                {
                    case "Customer":
                        var customer = context.Customers.Find(userId);
                        UserWrapper = new UserWrapper(customer!);
                        break;
                    case "Employee":
                        var employee = context.Employees.Find(userId);
                        UserWrapper = new UserWrapper(employee!);
                        break;
                    default:
                        UserWrapper = null!;
                        break;
                }
            }
        }
        else
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

        if (UserWrapper != null)
        {
            var order = basketService.PlaceOrder(UserWrapper);
            return RedirectToPage("OrderSuccess", new { orderId = order.Id });
        }
        else
        {
            return RedirectToPage("/Error");
        }
    }
    public IActionResult OnPostCancel()
    {
        return RedirectToPage("/Products/Index");
    }
}
namespace HttpWebshopCookie.Pages.Basket;

public class ReviewInfoModel(BasketService basketService, ApplicationDbContext context, IEmailService emailService, IOptions<SmtpSettings> smtpSettings) : PageModel
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
            string orderEmail = ReviewInfo.Email;
            if (order == null || string.IsNullOrEmpty(orderEmail))
            {
                return RedirectToPage("/Error");
            }
            else
            {
                MimeMessage message = BuildOrderConfirmationEmail(order, ReviewInfo);
                emailService.SendMimeMessageAsync(message);
                return RedirectToPage("OrderSuccess", new { orderId = order.Id });
            }
        }
        else
        {
            return RedirectToPage("/Error");
        }
    }
    public IActionResult OnPostCancel()
    {
        return RedirectToPage("/CustomerProducts/Index");
    }
    public MimeMessage BuildOrderConfirmationEmail(Order order, ReviewInfoViewModel reviewInfo)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(smtpSettings.Value.SenderName, smtpSettings.Value.SenderEmail));
        message.To.Add(new MailboxAddress("", reviewInfo.Email));
        message.Subject = "Order Confirmation";

        var bodyBuilder = new BodyBuilder();

        var sb = new StringBuilder();
        sb.AppendLine("<html lang=\"da\">");
        sb.AppendLine("<head>");
        sb.AppendLine("<meta charset=\"UTF-8\">");
        sb.AppendLine("<meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\">");
        sb.AppendLine("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
        sb.AppendLine("<title>Order Confirmation</title>");
        sb.AppendLine("</head>");
        sb.AppendLine("<body>");
        sb.AppendLine("<h2>Order Confirmation</h2>");
        sb.AppendLine("<p>Tak for din ordre! Vi har registreret følgende:</p>");
        sb.AppendLine("<h3>Order Details</h3>");
        sb.AppendLine("<table>");
        sb.AppendLine("<tr><td><strong>Order ID:</strong></td><td>" + order.Id + "</td></tr>");
        sb.AppendLine("<tr><td><strong>Order Date:</strong></td><td>" + order.OrderDate.ToString("f") + "</td></tr>");

        if (!string.IsNullOrEmpty(reviewInfo.FirstName) || !string.IsNullOrEmpty(reviewInfo.LastName))
        {
            sb.AppendLine("<tr><td><strong>Name:</strong></td><td>" + reviewInfo.FirstName + " " + reviewInfo.LastName + "</td></tr>");
        }
        if (!string.IsNullOrEmpty(reviewInfo.Email))
        {
            sb.AppendLine("<tr><td><strong>Email:</strong></td><td>" + reviewInfo.Email + "</td></tr>");
        }
        if (!string.IsNullOrEmpty(reviewInfo.PhoneNumber))
        {
            sb.AppendLine("<tr><td><strong>Phone:</strong></td><td>" + reviewInfo.PhoneNumber + "</td></tr>");
        }
        if (!string.IsNullOrEmpty(reviewInfo.Resident))
        {
            sb.AppendLine("<tr><td><strong>Resident:</strong></td><td>" + reviewInfo.Resident + "</td></tr>");
        }
        if (!string.IsNullOrEmpty(reviewInfo.Street))
        {
            sb.AppendLine("<tr><td><strong>Street:</strong></td><td>" + reviewInfo.Street + "</td></tr>");
        }
        if (!string.IsNullOrEmpty(reviewInfo.PostalCode))
        {
            sb.AppendLine("<tr><td><strong>Postal Code:</strong></td><td>" + reviewInfo.PostalCode + "</td></tr>");
        }
        if (!string.IsNullOrEmpty(reviewInfo.City))
        {
            sb.AppendLine("<tr><td><strong>City:</strong></td><td>" + reviewInfo.City + "</td></tr>");
        }
        if (!string.IsNullOrEmpty(reviewInfo.Country))
        {
            sb.AppendLine("<tr><td><strong>Country:</strong></td><td>" + reviewInfo.Country + "</td></tr>");
        }

        sb.AppendLine("</table></br>");
        sb.AppendLine("<h3>Order Items</h3>");
        sb.AppendLine("<table>");
        sb.AppendLine("<thead><tr><th>Product</th><th>Quantity</th><th>Price</th></tr></thead>");
        sb.AppendLine("<tbody>");
        foreach (var item in order.OrderItems)
        {
            sb.AppendLine("<tr>");
            sb.AppendLine("<td>" + item.ProductItem?.Name + "</td>");
            sb.AppendLine("<td>" + item.Quantity + "</td>");
            sb.AppendLine("<td>" + item.UnitPrice.ToString("C") + "</td>");
            sb.AppendLine("</tr>");
        }
        sb.AppendLine("</tbody>");
        sb.AppendLine("</table></br>");
        sb.AppendLine("<img src=\"cid:logo\" alt=\"Logo\" width=\"100\" height=\"100\" />");
        sb.AppendLine("<p>Tak for at lade BlomsterBinderiet levere din blomsteroplevelse til dig!</p>");
        sb.AppendLine("<p>Med venlig hilsen</p>");
        sb.AppendLine("<p>BlomsterBinderiet</p>");
        sb.AppendLine("</body>");
        sb.AppendLine("</html>");

        bodyBuilder.HtmlBody = sb.ToString();

        var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "logo-no-background.png");
        var logo = bodyBuilder.LinkedResources.Add(logoPath.Replace("/", "\\"));
        logo.ContentId = "logo";

        message.Body = bodyBuilder.ToMessageBody();

        return message;
    }
}

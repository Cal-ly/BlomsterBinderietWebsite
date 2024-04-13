namespace HttpWebshopCookie.Services;

public class BasketService(IHttpContextAccessor httpContextAccessor, ApplicationDbContext context, UserManager<IdentityUser> userManager)
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    private readonly ApplicationDbContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly UserManager<IdentityUser> _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));

    // Retrieve or create a new basket based on the basket ID stored in the cookie
    public Basket GetOrCreateBasket()
    {
        string? basketId = _httpContextAccessor.HttpContext?.Request.Cookies["BasketId"];
        Basket? basket;

        if (string.IsNullOrEmpty(basketId))
        {
            // Create a new basket if one does not exist
            basket = new Basket();
            _context.Baskets.Add(basket);
            _context.SaveChanges();
            StoreBasketIdInCookie(basket.Id);
        }
        else
        {
            // Fetch the existing basket from the database
            basket = _context.Baskets.Include(b => b.Items)
                                     .ThenInclude(i => i.ProductInBasket)
                                     .FirstOrDefault(b => b.Id == basketId);
            if (basket == null)
            {
                // Handle cases where the basket might not be found due to data inconsistency or deletion
                basket = new Basket();
                _context.Baskets.Add(basket);
                _context.SaveChanges();
                StoreBasketIdInCookie(basket.Id);
            }
        }

        return basket;
    }

    // Store the basket ID in the cookie with appropriate security settings
    private void StoreBasketIdInCookie(string basketId)
    {
        var options = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(30)  // Extend or modify based on the application needs
        };
        _httpContextAccessor.HttpContext?.Response.Cookies.Append("BasketId", basketId, options);
    }

    public async Task AddToBasket(string productId)
    {
        var basket = GetOrCreateBasket();
        var item = basket.Items.Find(i => i.ProductId == productId);
        if (item == null)
        {
            Product? product = _context.Products.Find(productId);
            if (product != null)
            {
                item = new BasketItem
                {
                    ProductId = productId,
                    Quantity = 1,
                    ProductInBasket = product
                };
                basket.Items.Add(item);
            }
        }
        else
        {
            item.Quantity++;
        }
        await _context.SaveChangesAsync();
        await LogBasketActivity(basket.Id, productId, "Add", item?.Quantity);
    }

    public async Task RemoveFromBasket(string productId)
    {
        var basket = GetOrCreateBasket();
        var item = basket.Items.Find(i => i.ProductId == productId);
        if (item != null)
        {
            if (item.Quantity > 1)
            {
                item.Quantity--;
            }
            else
            {
                basket.Items.Remove(item);
            }
            await _context.SaveChangesAsync();
            await LogBasketActivity(basket.Id, productId, "Remove", item?.Quantity);
        }
    }

    // Log activities for analytics purposes
    private async Task LogBasketActivity(string basketId, string productId, string activityType, int? quantityChanged)
    {
        var activity = new BasketActivity
        {
            BasketId = basketId,
            ProductId = productId,
            ActivityType = activityType,
            QuantityChanged = quantityChanged ?? 0,
            SessionId = _httpContextAccessor.HttpContext?.Session.Id,
            UserId = _userManager.GetUserId(_httpContextAccessor!.HttpContext?.User!) ?? null,
        };

        _context.BasketActivities.Add(activity);
        await _context.SaveChangesAsync();
    }
}

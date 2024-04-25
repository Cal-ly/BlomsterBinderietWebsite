using HttpWebshopCookie.Interfaces;

namespace HttpWebshopCookie.Services;

public class BasketService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IOrderCreator _orderCreator;

    public BasketService(IHttpContextAccessor httpContextAccessor, ApplicationDbContext context, UserManager<IdentityUser> userManager, IOrderCreator orderCreator) //TODO: convert to primary constructor
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _orderCreator = orderCreator;
    }

    public Basket GetOrCreateBasket()
    {
        string? basketId = _httpContextAccessor.HttpContext?.Request.Cookies["BasketId"];
        Basket? basket;

        if (string.IsNullOrEmpty(basketId))
        {
            basket = new Basket();
            _context.Baskets.Add(basket);
            _context.SaveChanges();
            StoreBasketIdInCookie(basket.Id);
        }
        else
        {
            basket = _context.Baskets.Include(b => b.Items)
                                     .ThenInclude(i => i.ProductInBasket)
                                     .FirstOrDefault(b => b.Id == basketId);
            if (basket == null)
            {
                basket = new Basket();
                _context.Baskets.Add(basket);
                _context.SaveChanges();
                StoreBasketIdInCookie(basket.Id);
            }
        }

        return basket;
    }

    private void StoreBasketIdInCookie(string basketId)
    {
        var options = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(30)
        };
        _httpContextAccessor.HttpContext?.Response.Cookies.Append("BasketId", basketId, options);
    }

    public async Task AddToBasket(string productId)
    {
        var basket = GetOrCreateBasket();
        var item = basket.Items.Find(i => i.ProductId == productId);

        if (item == null)
        {
            Product? product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                throw new InvalidOperationException("Product not found.");
            }

            item = new BasketItem
            {
                ProductId = productId,
                Quantity = 1,
                ProductInBasket = product
            };
            basket.Items.Add(item);
        }
        else
        {
            item.Quantity++;
        }

        await _context.SaveChangesAsync();
        await LogBasketActivity(basket.Id, productId, "Add", item?.Quantity);
    }

    public async Task<int> GetQuantityInBasket(string productId)
    {
        var basket = GetOrCreateBasket();
        var item = basket.Items.Find(i => i.ProductId == productId);
        int itemQuantity = item?.Quantity ?? 0;
        return await Task.FromResult(itemQuantity);
    }

    public async Task<Dictionary<string, int>> GetAllQuantitiesInBasket()
    {
        var basket = GetOrCreateBasket();

        if (basket.Id == null)
        {
            return new Dictionary<string, int>();
        }

        return await _context.BasketItems
            .Where(item => item.BasketId == basket.Id && item.ProductId != null)
            .Select(item => new { item.ProductId, item.Quantity })
            .Where(item => item.ProductId != null)
            .ToDictionaryAsync(item => item.ProductId!, item => item.Quantity ?? 0);
    }

    public async Task UpdateBasketItemQuantity(string productId, int newQuantity)
    {
        var basket = GetOrCreateBasket();
        var item = basket.Items.FirstOrDefault(i => i.ProductId == productId);
        if (item == null)
        {
            throw new InvalidOperationException("Product not in basket.");
        }

        if (newQuantity <= 0)
        {
            basket.Items.Remove(item);
        }
        else
        {
            item.Quantity = newQuantity;
        }

        await _context.SaveChangesAsync();
        await LogBasketActivity(basket.Id, productId, "Update", newQuantity);
    }

    public async Task RemoveFromBasket(string productId)
    {
        var basket = GetOrCreateBasket();
        var item = basket.Items.Find(i => i.ProductId == productId);

        if (item == null)
        {
            throw new InvalidOperationException("Product not in basket.");
        }

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

    public async Task ClearBasket()
    {
        var basket = GetOrCreateBasket();
        basket.Items.Clear();
        await _context.SaveChangesAsync();
        await LogBasketActivity(basket.Id, null, "ClearAll", 0);
    }

    private async Task LogBasketActivity(string basketId, string? productId, string activityType, int? quantityChanged)
    {
        var activity = new BasketActivity
        {
            BasketId = basketId,
            ProductId = productId,
            ActivityType = activityType,
            QuantityChanged = quantityChanged ?? 0,
            SessionId = _httpContextAccessor.HttpContext?.Session.Id,
            UserId = _userManager.GetUserId(_httpContextAccessor.HttpContext?.User!),
        };

        _context.BasketActivities.Add(activity);
        await _context.SaveChangesAsync();
    }

    public Order Checkout(UserWrapper userWrapper)
    {
        var basket = GetOrCreateBasket();
        var order = _orderCreator.CreateOrderFromBasket(basket, userWrapper);

        if (userWrapper != null)
        {
            var customer = _context.Customers.FirstOrDefault(c => c.Id == userWrapper.Id);
            if (customer != null)
            {
                order.CustomerId = customer.Id;
                order.Customer = customer;
            }
        }

        basket.Items.Clear();
        LogBasketActivity(basket.Id, null, "Checkout", 0).Wait();
        _context.SaveChanges();

        return order;
    }

    public List<BasketItem> ListBasketItems()
    {
        var basket = GetOrCreateBasket();
        return basket.Items.ToList();
    }
    //TODO: Might remove this method, as it is not used in the project
    public decimal CalculateTotalPrice()
    {
        var basket = GetOrCreateBasket();
        return basket.Items.Sum(item => item.LinePrice());
    }
}
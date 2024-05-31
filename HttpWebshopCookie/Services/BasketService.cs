namespace HttpWebshopCookie.Services;

/// <summary>
/// Service class for managing the shopping basket.
/// </summary>
public class BasketService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ApplicationDbContext _context;
    private readonly OrderService orderCreator;

    /// <summary>
    /// Initializes a new instance of the <see cref="BasketService"/> class.
    /// </summary>
    /// <param name="httpContextAccessor">The HTTP context accessor.</param>
    /// <param name="context">The application database context.</param>
    /// <param name="orderCreator">The order service for creating orders.</param>
    public BasketService(IHttpContextAccessor httpContextAccessor, ApplicationDbContext context, OrderService orderCreator)
    {
        this.orderCreator = orderCreator;
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Gets or creates the shopping basket for the current HTTP context.
    /// </summary>
    /// <returns>The shopping basket.</returns>
    public Basket GetOrCreateBasket()
    {
        return GetOrCreateBasket(_httpContextAccessor, _context);
    }

    /// <summary>
    /// Gets or creates the shopping basket for the specified HTTP context and database context.
    /// </summary>
    /// <param name="httpContextAccessor">The HTTP context accessor.</param>
    /// <param name="dbContext">The application database context.</param>
    /// <returns>The shopping basket.</returns>
    public Basket GetOrCreateBasket(IHttpContextAccessor httpContextAccessor, ApplicationDbContext dbContext)
    {
        return GetOrCreateBasketInternal(httpContextAccessor, dbContext);
    }

    /// <summary>
    /// Internal method to get or create the shopping basket.
    /// </summary>
    /// <param name="httpContextAccessor">The HTTP context accessor.</param>
    /// <param name="dbContext">The application database context.</param>
    /// <returns>The shopping basket.</returns>
    private Basket GetOrCreateBasketInternal(IHttpContextAccessor httpContextAccessor, ApplicationDbContext dbContext)
    {
        string? basketId = httpContextAccessor.HttpContext?.Request.Cookies["BasketId"];
        Basket? basket;

        if (string.IsNullOrEmpty(basketId))
        {
            basket = new Basket();
            dbContext.Baskets.Add(basket);
            dbContext.SaveChanges();
            StoreBasketIdInCookie(basket.Id, httpContextAccessor);
        }
        else
        {
            basket = dbContext.Baskets.Include(b => b.Items)
                                      .ThenInclude(i => i.ProductInBasket)
                                      .FirstOrDefault(b => b.Id == basketId);
            if (basket == null)
            {
                basket = new Basket();
                dbContext.Baskets.Add(basket);
                dbContext.SaveChanges();
                StoreBasketIdInCookie(basket.Id, httpContextAccessor);
            }
        }

        return basket;
    }

    /// <summary>
    /// Stores the basket ID in a cookie for the current HTTP context.
    /// </summary>
    /// <param name="basketId">The ID of the basket.</param>
    /// <param name="httpContextAccessor">The HTTP context accessor.</param>
    private void StoreBasketIdInCookie(string basketId, IHttpContextAccessor httpContextAccessor)
    {
        var options = new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Lax,
            Expires = DateTime.UtcNow.AddDays(1)
        };
        httpContextAccessor.HttpContext?.Response.Cookies.Append("BasketId", basketId, options);
    }

    /// <summary>
    /// Adds a product to the shopping basket.
    /// </summary>
    /// <param name="productId">The ID of the product to add.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the specified product is not found.</exception>
    public async Task AddToBasket(string productId)
    {
        var basket = GetOrCreateBasket();
        var item = basket.Items.FirstOrDefault(i => i.ProductId == productId);

        if (item != null)
        {
            item.Quantity++;
        }
        else
        {
            Product? product = await _context.Products.FindAsync(productId) ?? throw new InvalidOperationException("Product not found.");
            item = new BasketItem
            {
                ProductId = productId,
                Quantity = 1,
                ProductInBasket = product
            };
            basket.Items.Add(item);
        }

        await _context.SaveChangesAsync();
        await LogBasketActivity(basket.Id, productId, "Add", item?.Quantity);
    }

    /// <summary>
    /// Checks if a product is in the shopping basket.
    /// </summary>
    /// <param name="productId">The ID of the product to check.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains a boolean indicating if the product is in the basket.</returns>
    public async Task<bool> IsInBasket(string productId)
    {
        var basket = GetOrCreateBasket();
        return await Task.FromResult(basket.Items.Any(i => i.ProductId == productId));
    }

    /// <summary>
    /// Gets the quantity of a product in the shopping basket.
    /// </summary>
    /// <param name="productId">The ID of the product to get the quantity for.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the quantity of the product in the basket.</returns>
    public async Task<int?> GetQuantityInBasket(string productId)
    {
        var basket = GetOrCreateBasket();
        var item = basket.Items.FirstOrDefault(i => i.ProductId == productId);

        if (item == null)
        {
            return 0;
        }

        return await Task.FromResult(item.Quantity);
    }

    /// <summary>
    /// Gets the quantities of all products in the shopping basket.
    /// </summary>
    /// <returns>A task representing the asynchronous operation. The task result contains a dictionary with the product IDs as keys and the quantities as values.</returns>
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

    /// <summary>
    /// Updates the quantity of a product in the shopping basket.
    /// </summary>
    /// <param name="productId">The ID of the product to update.</param>
    /// <param name="newQuantity">The new quantity of the product.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the specified product is not found in the basket.</exception>
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

    /// <summary>
    /// Removes a product from the shopping basket.
    /// </summary>
    /// <param name="productId">The ID of the product to remove.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task RemoveFromBasket(string productId)
    {
        var basket = GetOrCreateBasket();
        var item = basket.Items.FirstOrDefault(i => i.ProductId == productId);

        if (item == null)
        {
            return;
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

    /// <summary>
    /// Clears the shopping basket.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task ClearBasket()
    {
        var basket = GetOrCreateBasket();
        basket.Items.Clear();
        await _context.SaveChangesAsync();
        await LogBasketActivity(basket.Id, null, "ClearAll", 0);
    }

    /// <summary>
    /// Logs an activity in the basket.
    /// </summary>
    /// <param name="basketId">The ID of the basket.</param>
    /// <param name="productId">The ID of the product involved in the activity.</param>
    /// <param name="activityType">The type of activity (Add, Update, Remove, etc.).</param>
    /// <param name="quantityChanged">The quantity changed in the activity.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task LogBasketActivity(string basketId, string? productId, string activityType, int? quantityChanged)
    {
        var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var sessionId = _httpContextAccessor.HttpContext?.Session.Id;

        var activity = new BasketActivity
        {
            BasketId = basketId,
            ProductId = productId,
            ActivityType = activityType,
            QuantityChanged = quantityChanged ?? 0,
            SessionId = sessionId,
            UserId = string.IsNullOrEmpty(userId) ? null : userId,
            IsRegisteredUser = !string.IsNullOrEmpty(userId)
        };

        _context.BasketActivities.Add(activity);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Places an order using the shopping basket.
    /// </summary>
    /// <param name="userWrapper">The user wrapper for the order.</param>
    /// <returns>The order placed.</returns>
    public Order PlaceOrder(UserWrapper userWrapper)
    {
        var basket = GetOrCreateBasket();
        var order = orderCreator.CreateOrderFromBasket(basket, userWrapper);

        basket.Items.Clear();
        LogBasketActivity(basket.Id, null, "Checkout", 0).Wait();
        _context.SaveChanges();

        return order;
    }
}
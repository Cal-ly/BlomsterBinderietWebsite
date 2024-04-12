using Newtonsoft.Json;

namespace HttpWebshopCookie.Services;

public class BasketService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ApplicationDbContext _context;

    public BasketService(IHttpContextAccessor httpContextAccessor, ApplicationDbContext context)
    {
        _httpContextAccessor = httpContextAccessor;
        _context = context;
    }

    public Basket GetOrCreateBasket()
    {
        string? basketId = _httpContextAccessor.HttpContext?.Session.GetString("BasketId");
        Basket basket;

        if (basketId == null)
        {
            basket = new Basket();
            _context.Baskets.Add(basket);
            _context.SaveChanges();
            _httpContextAccessor.HttpContext?.Session.SetString("BasketId", basket.Id);
        }
        else
        {
            basket = _context.Baskets.Find(basketId) ?? new Basket();
        }

        return basket;
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
        _httpContextAccessor.HttpContext?.Response.Cookies.Append("basket", JsonConvert.SerializeObject(basket));
        await LogBasketActivity(basket.Id, productId, "Add", item?.Quantity);
    }

    public async Task RemoveFromBasket(string productId)
    {
        var basket = GetOrCreateBasket();
        var item = basket.Items.Find(i => i.ProductId == productId);
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
        _httpContextAccessor.HttpContext?.Response.Cookies.Append("basket", JsonConvert.SerializeObject(basket));
        await LogBasketActivity(basket.Id, productId, "Remove", item.Quantity);
    }

    private async Task LogBasketActivity(string basketId, string productId, string activityType, int? quantityChanged)
    {
        var activity = new BasketActivity
        {
            BasketId = basketId,
            ProductId = productId,
            ActivityType = activityType,
            QuantityChanged = quantityChanged ?? 0,
        };

        var userId = _httpContextAccessor.HttpContext?.User?.Identity?.Name;
        if (!string.IsNullOrEmpty(userId))
        {
            activity.UserId = userId;
        }

        activity.SessionId = _httpContextAccessor.HttpContext?.Session.Id;

        _context.BasketActivities.Add(activity);
        await _context.SaveChangesAsync();
    }
}

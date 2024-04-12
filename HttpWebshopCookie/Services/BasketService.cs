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

    public Basket GetBasket()
    {
        var basket = _httpContextAccessor.HttpContext?.Request.Cookies["basket"];
        if (string.IsNullOrEmpty(basket))
        {
            return new Basket();
        }

        try
        {
            var deserializedBasket = JsonConvert.DeserializeObject<Basket>(basket);
            return deserializedBasket ?? new Basket();
        }
        catch (JsonException)
        {
            return new Basket();
        }
    }

    public void AddToBasket(string productId)
    {
        var basket = GetBasket();
        var item = basket.Items.Find(i => i.ProductId == productId);
        if (item == null)
        {
            Product? product = _context.Products.FirstOrDefault(p => p.Id == productId);
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
    }

    public void RemoveFromBasket(string productId)
    {
        var basket = GetBasket();
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
    }
}

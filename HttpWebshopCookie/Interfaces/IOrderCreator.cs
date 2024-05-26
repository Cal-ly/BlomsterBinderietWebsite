using HttpWebshopCookie.Utilities;

namespace HttpWebshopCookie.Interfaces;

public interface IOrderCreator
{
    Order CreateOrderFromBasket(Basket basket, UserWrapper userWrapper);
}
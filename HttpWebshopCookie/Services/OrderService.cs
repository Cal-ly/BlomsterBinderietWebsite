namespace HttpWebshopCookie.Services;

public class OrderService(ApplicationDbContext context) : IOrderCreator
{
    public Order CreateOrderFromBasket(Basket basket, UserWrapper userWrapper)
    {
        var order = new Order
        {
            OrderDate = DateTime.Now,
            Status = OrderStatus.Pending,
        };

        foreach (var basketItem in basket.Items)
        {
            var orderItem = new OrderItem
            {
                ProductItem = basketItem.ProductInBasket,
                ProductId = basketItem.ProductId,
                Quantity = basketItem.Quantity ?? 0,
                UnitPrice = basketItem.ProductInBasket?.Price ?? 0
            };

            order.OrderItems.Add(orderItem);
        }

        if (userWrapper.Guest != null)
        {
            order.GuestUser = userWrapper.Guest;
            order.GuestUserId = userWrapper.Id;
        }
        else if (userWrapper.Customer != null)
        {
            order.Customer = userWrapper.Customer;
            order.CustomerId = userWrapper.Id;
        }
        else
        {
            throw new ArgumentException("User or GuestUser must be provided");
        }

        context.Orders.Add(order);
        context.SaveChanges();

        return order;
    }
}
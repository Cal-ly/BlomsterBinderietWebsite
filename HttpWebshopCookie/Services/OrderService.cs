namespace HttpWebshopCookie.Services;

public class OrderService : IOrderCreator
{
    private readonly ApplicationDbContext _context;

    public OrderService(ApplicationDbContext context)
    {
        _context = context;
    }

    public Order CreateOrderFromBasket(Basket basket)
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

        _context.Orders.Add(order);
        _context.SaveChanges();

        return order;
    }
}
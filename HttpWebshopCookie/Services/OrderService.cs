namespace HttpWebshopCookie.Services;

public class OrderService(ApplicationDbContext context)
{
    public Order CreateOrderFromBasket(Basket basket, UserWrapper userWrapper)
    {
        var order = new Order
        {
            OrderDate = DateTime.Now,
            Status = OrderStatus.Pending,
        };

        // Add each basket item to the order as order items
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

        // Set the user-related information on the order based on user type
        switch (userWrapper.GetUserType())
        {
            case "Guest":
                order.Guest = userWrapper.Guest;
                order.GuestId = userWrapper.Id;
                context.GuestUsers.Add(userWrapper.Guest!);
                break;
            case "Customer":
                order.Customer = userWrapper.Customer;
                order.CustomerId = userWrapper.Id;
                break;
            case "Employee":
                order.Employee = userWrapper.Employee;
                order.EmployeeId = userWrapper.Id;
                break;
            default:
                throw new ArgumentException("Valid user type must be provided");
        }

        context.Orders.Add(order);
        context.SaveChanges();

        return order;
    }
}
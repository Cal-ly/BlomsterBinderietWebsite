namespace HttpWebshopCookie.Services;

public class OrderService(ApplicationDbContext context)
{
    public Order GetOrder(string orderId)
    {
        return context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.ProductItem)
            .SingleOrDefault(o => o.Id == orderId)!;
    }

    public OrderItemView[] GetOrderItems(Order order)
    {
        return order.OrderItems.Select(oi => new OrderItemView
        {
            ProductName = oi.ProductItem?.Name,
            Quantity = oi.Quantity,
            Price = oi.ProductItem?.Price
        }).ToArray();
    }

    public string GetTotalPriceString(Order order)
    {
        return order.TotalPrice.ToString("C2");
    }

    public void UpdateOrderStatus(string orderId, OrderStatus newStatus)
    {
        var order = context.Orders.Find(orderId);
        if (order == null)
        {
            throw new InvalidOperationException("Order not found.");
        }

        order.Status = newStatus;
        context.SaveChanges();
    }

    public void UpdateOrder(Order order)
    {
        context.Orders.Update(order);
        context.SaveChanges();
    }

    public void DeleteOrder(string orderId)
    {
        var order = context.Orders.Find(orderId);
        if (order == null)
        {
            throw new InvalidOperationException("Order not found.");
        }

        context.Orders.Remove(order);
        context.SaveChanges();
    }

    public List<UserWrapper> GetOrderInvolved(Order order)
    {
        List<UserWrapper> involvedEntities = new();

        // Add any other entities involved with the order
        if (order.Guest != null)
        {
            var guestWrapper = new UserWrapper(order.Guest);
            involvedEntities.Add(guestWrapper);
        }

        if (order.Customer != null)
        {
            var customerWrapper = new UserWrapper(order.Customer);
            involvedEntities.Add(customerWrapper);
        }

        if (order.Employee != null)
        {
            var employeeWrapper = new UserWrapper(order.Employee);
            involvedEntities.Add(employeeWrapper);
        }

        return involvedEntities;
    }

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
                ProductId = basketItem.ProductId!,
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
                context.Addresses.Add(userWrapper.Guest!.Address!);
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
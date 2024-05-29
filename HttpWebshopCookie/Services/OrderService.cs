namespace HttpWebshopCookie.Services
{
    /// <summary>
    /// Service class for managing orders.
    /// </summary>
    public class OrderService
    {
        private readonly ApplicationDbContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderService"/> class.
        /// </summary>
        /// <param name="context">The application database context.</param>
        public OrderService(ApplicationDbContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context), "Database context cannot be null.");
        }

        /// <summary>
        /// Retrieves an order by its ID.
        /// </summary>
        /// <param name="orderId">The ID of the order to retrieve.</param>
        /// <returns>The order with the specified ID.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the order is not found.</exception>
        public Order GetOrder(string orderId)
        {
            var order = context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.ProductItem)
                .SingleOrDefault(o => o.Id == orderId);

            return order ?? throw new InvalidOperationException("Order not found.");
        }

        /// <summary>
        /// Retrieves the order items for a given order.
        /// </summary>
        /// <param name="order">The order for which to retrieve the order items.</param>
        /// <returns>An array of order item views.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the order is null.</exception>
        public OrderItemView[] GetOrderItems(Order order)
        {
            if (order == null) throw new ArgumentNullException(nameof(order), "Order cannot be null.");

            return order.OrderItems.Select(oi => new OrderItemView
            {
                ProductName = oi.ProductItem?.Name,
                Quantity = oi.Quantity,
                Price = oi.ProductItem?.Price
            }).ToArray();
        }

        /// <summary>
        /// Retrieves the total price of an order as a formatted string.
        /// </summary>
        /// <param name="order">The order for which to retrieve the total price.</param>
        /// <returns>The total price of the order as a formatted string.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the order is null.</exception>
        public string GetTotalPriceString(Order order)
        {
            if (order == null) throw new ArgumentNullException(nameof(order), "Order cannot be null.");

            return order.TotalPrice.ToString("C2");
        }

        /// <summary>
        /// Updates the status of an order.
        /// </summary>
        /// <param name="orderId">The ID of the order to update.</param>
        /// <param name="newStatus">The new status of the order.</param>
        /// <exception cref="InvalidOperationException">Thrown when the order is not found.</exception>
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

        /// <summary>
        /// Updates an order.
        /// </summary>
        /// <param name="order">The order to update.</param>
        /// <exception cref="ArgumentNullException">Thrown when the order is null.</exception>
        public void UpdateOrder(Order order)
        {
            if (order == null) throw new ArgumentNullException(nameof(order), "Order cannot be null.");

            context.Orders.Update(order);
            context.SaveChanges();
        }

        /// <summary>
        /// Deletes an order.
        /// </summary>
        /// <param name="orderId">The ID of the order to delete.</param>
        /// <exception cref="InvalidOperationException">Thrown when the order is not found.</exception>
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

        /// <summary>
        /// Retrieves the users involved in an order.
        /// </summary>
        /// <param name="order">The order for which to retrieve the involved users.</param>
        /// <returns>A list of user wrappers representing the involved users.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the order is null.</exception>
        public List<UserWrapper> GetOrderInvolved(Order order)
        {
            if (order == null) throw new ArgumentNullException(nameof(order), "Order cannot be null.");

            List<UserWrapper> involvedEntities = new();

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

        /// <summary>
        /// Creates an order from a basket.
        /// </summary>
        /// <param name="basket">The basket from which to create the order.</param>
        /// <param name="userWrapper">The user wrapper associated with the order.</param>
        /// <returns>The created order.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the basket or userWrapper is null.</exception>
        /// <exception cref="ArgumentException">Thrown when an invalid user type is provided.</exception>
        public Order CreateOrderFromBasket(Basket basket, UserWrapper userWrapper)
        {
            if (basket == null) throw new ArgumentNullException(nameof(basket), "Basket cannot be null.");
            if (userWrapper == null) throw new ArgumentNullException(nameof(userWrapper), "User wrapper cannot be null.");

            var order = new Order
            {
                OrderDate = DateTime.Now,
                Status = OrderStatus.Pending,
            };

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
                case "ApplicationUser":
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

            context.Orders.Add(order);
            context.SaveChanges();
            return order;
        }
    }
}

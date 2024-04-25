namespace HttpWebshopCookie.Pages.Basket;

public class OrderSuccessModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public string OrderId { get; set; }
    public DateTime EstimatedDeliveryDate { get; set; }
    public List<OrderItemView> OrderItems { get; set; }
    public decimal Total { get; set; }

    public OrderSuccessModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public void OnGet(string orderId)
    {
        var order = _context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.ProductItem)
            .SingleOrDefault(o => o.Id == orderId);

        if (order == null)
        {
            RedirectToPage("/Error");
        }

        OrderId = orderId;
        OrderItems = order.OrderItems.Select(oi => new OrderItemView
        {
            ProductName = oi.ProductItem.Name,
            Quantity = oi.Quantity,
            Price = oi.ProductItem.Price
        }).ToList();

        Total = OrderItems.Sum(oi => oi.Price * oi.Quantity);
        EstimatedDeliveryDate = DateTime.Now.AddDays(3); // Example: Adding three days for delivery.
    }

    public class OrderItemView
    {
        public string? ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}

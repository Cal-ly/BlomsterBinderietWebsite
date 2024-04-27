namespace HttpWebshopCookie.Pages.Basket;

public class OrderSuccessModel(ApplicationDbContext context) : PageModel
{
    public string? OrderId { get; set; }
    public List<OrderItemView>? OrderItems { get; set; } = [];
    public string? TotalString { get; set; }

    public void OnGet(string orderId)
    {
        var order = context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.ProductItem)
            .SingleOrDefault(o => o.Id == orderId);

        if (order == null)
        {
            RedirectToPage("/Error");
        }

        OrderId = orderId;
        OrderItems = order!.OrderItems.Select(oi => new OrderItemView
        {
            ProductName = oi.ProductItem?.Name,
            Quantity = oi.Quantity,
            Price = oi.ProductItem?.Price
        }).ToList();

        TotalString = order?.TotalPrice.ToString("C2");
    }

    public class OrderItemView
    {
        public string? ProductName { get; set; }
        public int? Quantity { get; set; }
        public decimal? Price { get; set; }
    }
}

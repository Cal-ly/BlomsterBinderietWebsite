namespace HttpWebshopCookie.Pages.Basket;

public class OrderSuccessModel(ApplicationDbContext context, OrderService orderService) : PageModel
{
    public string? OrderId { get; set; }
    public List<OrderItemView>? OrderItems { get; set; } = [];
    public string? TotalString { get; set; }

    public IActionResult OnGet(string orderId)
    {
        var order = context.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.ProductItem)
            .SingleOrDefault(o => o.Id == orderId);

        if (order == null)
        {
            return RedirectToPage("/Error");
        }

        OrderId = orderId;

        OrderItems = [.. orderService.GetOrderItems(order!)];

        TotalString = order.TotalPrice.ToString("C");

        return Page();
    }
}
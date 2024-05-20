namespace HttpWebshopCookie.Pages.Admin.Analytics;

[Authorize(Policy = "managerAccess")]
public class ProductsOrdersModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public ProductsOrdersModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public ProductsOrdersData Data { get; set; }
    public string Period { get; set; }

    public async Task OnGetAsync(string period = "Month")
    {
        Period = period;
        Data = new ProductsOrdersData
        {
            TopProducts = await GetTopProductsAsync(period),
            OrderStatusBreakdown = await GetOrderStatusBreakdownAsync(period),
            AvgTimeToFulfillOrders = await GetAverageTimeToFulfillOrdersAsync(period)
        };
    }

    private async Task<List<TopProduct>> GetTopProductsAsync(string period)
    {
        var query = _context.OrderItems
            .Include(oi => oi.Order)
            .Where(oi => oi.Order.Status == OrderStatus.Completed);

        DateTime currentDate = DateTime.UtcNow.Date;
        switch (period)
        {
            case "Day":
                query = query.Where(oi => oi.Order.OrderDate.Date == currentDate);
                break;
            case "Month":
                query = query.Where(oi => oi.Order.OrderDate.Year == currentDate.Year && oi.Order.OrderDate.Month == currentDate.Month);
                break;
            case "Year":
                query = query.Where(oi => oi.Order.OrderDate.Year == currentDate.Year);
                break;
        }

        var topProducts = await query
            .GroupBy(oi => new { oi.ProductId, oi.ProductItem.Name })
            .Select(g => new TopProduct { ProductName = g.Key.Name ?? "Unknown", Sales = g.Sum(oi => oi.Quantity * oi.UnitPrice) })
            .OrderByDescending(tp => tp.Sales)
            .Take(10)
            .ToListAsync();

        return topProducts;
    }

    private async Task<List<OrderStatusBreakdown>> GetOrderStatusBreakdownAsync(string period)
    {
        IQueryable<Order> query = _context.Orders;
        switch (period)
        {
            case "Day":
                query = query.Where(o => o.OrderDate.Date == DateTime.UtcNow.Date);
                break;
            case "Month":
                query = query.Where(o => o.OrderDate.Month == DateTime.UtcNow.Month && o.OrderDate.Year == DateTime.UtcNow.Year);
                break;
            case "Year":
                query = query.Where(o => o.OrderDate.Year == DateTime.UtcNow.Year);
                break;
        }

        return await query.GroupBy(o => o.Status)
                  .Select(g => new OrderStatusBreakdown { Status = g.Key.ToString() ?? "Unknown", Count = g.Count() })
                  .ToListAsync();
    }

    private async Task<double> GetAverageTimeToFulfillOrdersAsync(string period)
    {
        IQueryable<Order> query = _context.Orders.Where(o => o.Status == OrderStatus.Completed);
        switch (period)
        {
            case "Day":
                query = query.Where(o => o.OrderDate.Date == DateTime.UtcNow.Date);
                break;
            case "Month":
                query = query.Where(o => o.OrderDate.Month == DateTime.UtcNow.Month && o.OrderDate.Year == DateTime.UtcNow.Year);
                break;
            case "Year":
                query = query.Where(o => o.OrderDate.Year == DateTime.UtcNow.Year);
                break;
        }

        return await query.AverageAsync(o => EF.Functions.DateDiffDay(o.OrderDate, o.CompletionDate ?? o.OrderDate));
    }

    public class ProductsOrdersData
    {
        public List<TopProduct> TopProducts { get; set; } = new List<TopProduct>();
        public List<OrderStatusBreakdown> OrderStatusBreakdown { get; set; } = new List<OrderStatusBreakdown>();
        public double AvgTimeToFulfillOrders { get; set; }
    }
}

public class TopProduct
{
    public string? ProductName { get; set; }
    public decimal Sales { get; set; }
}

public class OrderStatusBreakdown
{
    public string? Status { get; set; }
    public int Count { get; set; }
}

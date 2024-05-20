namespace HttpWebshopCookie.Pages.Admin.Analytics;

[Authorize(Policy = "managerAccess")]
public class ProductsOrdersModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ProductsOrdersModel> _logger;

    public ProductsOrdersModel(ApplicationDbContext context, ILogger<ProductsOrdersModel> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public ProductsOrdersData Data { get; set; } = new ProductsOrdersData();
    public string? Period { get; set; } = "Month";

    public async Task OnGetAsync(string period = "Month")
    {
        Period = period ?? "Month";

        try
        {
            Data.TopProducts = await GetTopProductsAsync(Period);
            Data.OrderStatusBreakdown = await GetOrderStatusBreakdownAsync(Period);
            Data.AvgTimeToFulfillOrders = await GetAverageTimeToFulfillOrdersAsync(Period);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching product and order data.");
            RedirectToPage("/Error");
        }
    }

    private IQueryable<Order> FilterOrdersByPeriod(IQueryable<Order> query, string period)
    {
        var now = DateTime.UtcNow;

        return period switch
        {
            "Day" => query.Where(o => o.OrderDate.Date == now.Date),
            "Month" => query.Where(o => o.OrderDate.Year == now.Year && o.OrderDate.Month == now.Month),
            "Year" => query.Where(o => o.OrderDate.Year == now.Year),
            _ => query
        };
    }

    private async Task<List<TopProduct>> GetTopProductsAsync(string period)
    {
        IQueryable<OrderItem> query = _context.OrderItems
            .Include(oi => oi.Order)
            .Where(oi => oi.Order!.Status == OrderStatus.Completed);

        query = FilterOrdersByPeriod(query.Select(oi => oi.Order!), period)
            .SelectMany(o => o.OrderItems);

        var topProducts = await query
            .GroupBy(oi => new { oi.ProductId, oi.ProductItem!.Name })
            .Select(g => new TopProduct
            {
                ProductName = g.Key.Name ?? "Unknown",
                Sales = g.Sum(oi => oi.Quantity * oi.UnitPrice)
            })
            .OrderByDescending(tp => tp.Sales)
            .Take(10)
            .ToListAsync();

        return topProducts;
    }

    private async Task<List<OrderStatusBreakdown>> GetOrderStatusBreakdownAsync(string period)
    {
        var query = FilterOrdersByPeriod(_context.Orders, period);

        return await query.GroupBy(o => o.Status)
                  .Select(g => new OrderStatusBreakdown
                  {
                      Status = g.Key.ToString() ?? "Unknown",
                      Count = g.Count()
                  })
                  .ToListAsync();
    }

    private async Task<double> GetAverageTimeToFulfillOrdersAsync(string period)
    {
        var query = FilterOrdersByPeriod(_context.Orders.Where(o => o.Status == OrderStatus.Completed), period);

        return await query.AverageAsync(o => EF.Functions.DateDiffDay(o.OrderDate, o.CompletionDate ?? o.OrderDate));
    }

    public class ProductsOrdersData
    {
        public List<TopProduct> TopProducts { get; set; } = new List<TopProduct>();
        public List<OrderStatusBreakdown> OrderStatusBreakdown { get; set; } = new List<OrderStatusBreakdown>();
        public double AvgTimeToFulfillOrders { get; set; }
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
}
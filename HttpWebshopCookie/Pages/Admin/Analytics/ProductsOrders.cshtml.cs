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
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }

    public async Task OnGetAsync(DateTime? dateFrom = null, DateTime? dateTo = null)
    {
        DateFrom = dateFrom ?? DateTime.UtcNow.AddYears(-1);
        DateTo = dateTo ?? DateTime.UtcNow;

        try
        {
            Data.TopProducts = await GetTopProductsAsync();
            Data.OrderStatusBreakdown = await GetOrderStatusBreakdownAsync();
            Data.AvgTimeToFulfillOrders = await GetAverageTimeToFulfillOrdersAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching product and order data.");
            RedirectToPage("/Error");
        }
    }

    private IQueryable<Order> FilterOrdersByDateRange(IQueryable<Order> query)
    {
        return query.Where(o => o.OrderDate >= DateFrom && o.OrderDate <= DateTo);
    }

    private async Task<List<TopProduct>> GetTopProductsAsync()
    {
        IQueryable<OrderItem> query = _context.OrderItems
            .Include(oi => oi.Order)
            .Where(oi => oi.Order!.Status == OrderStatus.Completed);

        query = FilterOrdersByDateRange(query.Select(oi => oi.Order!))
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

    private async Task<List<OrderStatusBreakdown>> GetOrderStatusBreakdownAsync()
    {
        var query = FilterOrdersByDateRange(_context.Orders);

        return await query.GroupBy(o => o.Status)
                  .Select(g => new OrderStatusBreakdown
                  {
                      Status = g.Key.ToString() ?? "Unknown",
                      Count = g.Count()
                  })
                  .ToListAsync();
    }

    private async Task<double> GetAverageTimeToFulfillOrdersAsync()
    {
        var query = FilterOrdersByDateRange(_context.Orders.Where(o => o.Status == OrderStatus.Completed));

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
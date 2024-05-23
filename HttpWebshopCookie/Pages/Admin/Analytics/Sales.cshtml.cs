namespace HttpWebshopCookie.Pages.Admin.Analytics;

[Authorize(Policy = "managerAccess")]
public class SalesModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<SalesModel> _logger;

    public SalesModel(ApplicationDbContext context, ILogger<SalesModel> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [BindProperty]
    public SalesData Data { get; set; } = new SalesData();
    [BindProperty(SupportsGet = true)]
    public DateTime DateFrom { get; set; } = DateTime.UtcNow.AddDays(-90);
    [BindProperty(SupportsGet = true)]
    public DateTime DateTo { get; set; } = DateTime.UtcNow;

    public async Task OnGetAsync(DateTime? dateFrom, DateTime? dateTo)
    {
        DateFrom = dateFrom ?? DateTime.UtcNow.AddDays(-90);
        DateTo = dateTo ?? DateTime.UtcNow;

        try
        {
            Data.TotalSales = await GetTotalSalesAsync();
            Data.TotalOrders = await GetTotalOrdersAsync();
            Data.AverageOrderValue = await GetAverageOrderValueAsync();
            Data.SalesGrowthRate = await GetSalesGrowthRateAsync();
            Data.AverageOrderProcessingTime = await GetAverageOrderProcessingTimeAsync();
            Data.TopSellingProducts = await GetTopSellingProductsAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching sales data.");
            RedirectToPage("/Error");
        }
    }

    private IQueryable<Order> FilterOrdersByDateRange(IQueryable<Order> query)
    {
        return query.Where(o => o.OrderDate >= DateFrom && o.OrderDate <= DateTo);
    }

    private async Task<decimal> GetTotalSalesAsync()
    {
        var query = _context.Orders.Where(o => o.Status == OrderStatus.Completed);
        query = FilterOrdersByDateRange(query);
        return await query.SumAsync(o => o.GetTotalPrice());
    }

    private async Task<int> GetTotalOrdersAsync()
    {
        IQueryable<Order> query = _context.Orders;
        query = FilterOrdersByDateRange(query);
        return await query.CountAsync();
    }

    private async Task<decimal> GetAverageOrderValueAsync()
    {
        IQueryable<Order> query = _context.Orders;
        query = FilterOrdersByDateRange(query);
        return await query.AverageAsync(o => o.GetTotalPrice());
    }

    private async Task<double> GetSalesGrowthRateAsync()
    {
        var previousPeriodEnd = DateFrom.AddDays(-1);
        var previousPeriodStart = previousPeriodEnd.AddDays(-(DateTo - DateFrom).TotalDays);
        var previousPeriodSales = await _context.Orders
            .Where(o => o.Status == OrderStatus.Completed && o.OrderDate >= previousPeriodStart && o.OrderDate <= previousPeriodEnd)
            .SumAsync(o => o.GetTotalPrice());

        var currentPeriodSales = await GetTotalSalesAsync();

        if (previousPeriodSales == 0) return 100;
        var salesGrowth = ((currentPeriodSales - previousPeriodSales) / previousPeriodSales) * 100;
        return (double)salesGrowth;
    }

    private async Task<double> GetAverageOrderProcessingTimeAsync()
    {
        var query = _context.Orders.Where(o => o.Status == OrderStatus.Completed);
        query = FilterOrdersByDateRange(query);
        return await query.AverageAsync(o => EF.Functions.DateDiffDay(o.OrderDate, o.CompletionDate ?? o.OrderDate));
    }

    private async Task<List<TopProduct>> GetTopSellingProductsAsync()
    {
        IQueryable<OrderItem> query = _context.OrderItems
            .Include(oi => oi.Order)
            .Where(oi => oi.Order!.Status == OrderStatus.Completed && oi.Order.OrderDate >= DateFrom && oi.Order.OrderDate <= DateTo);

        var topProducts = await query
            .GroupBy(oi => new { oi.ProductId, oi.ProductItem!.Name })
            .Select(g => new TopProduct
            {
                ProductName = g.Key.Name!,
                Sales = g.Sum(oi => oi.Quantity * oi.UnitPrice)
            })
            .OrderByDescending(tp => tp.Sales)
            .Take(5)
            .ToListAsync();

        return topProducts;
    }

    public class SalesData
    {
        public decimal TotalSales { get; set; }
        public int TotalOrders { get; set; }
        public decimal AverageOrderValue { get; set; }
        public double SalesGrowthRate { get; set; }
        public double AverageOrderProcessingTime { get; set; }
        public List<TopProduct> TopSellingProducts { get; set; } = new List<TopProduct>();
    }

    public class TopProduct
    {
        public string? ProductName { get; set; }
        public decimal Sales { get; set; }
    }
}

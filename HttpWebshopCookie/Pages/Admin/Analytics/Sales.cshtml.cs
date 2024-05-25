namespace HttpWebshopCookie.Pages.Admin.Analytics;

[Authorize(Policy = "managerAccess")]
public class SalesModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<SalesModel> _logger;
    private IQueryable<Order>? _query;

    public SalesModel(ApplicationDbContext context, ILogger<SalesModel> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [BindProperty]
    public SalesData Data { get; set; } = new SalesData();

    [BindProperty(SupportsGet = true)]
    public DateTime DateFrom { get; set; } = DateTime.Now.AddDays(-90);

    [BindProperty(SupportsGet = true)]
    public DateTime DateTo { get; set; } = DateTime.Now;

    public async Task OnGetAsync(DateTime? dateFrom, DateTime? dateTo)
    {
        DateFrom = dateFrom ?? DateTime.Now.AddDays(-90);
        DateTo = dateTo ?? DateTime.Now;

        try
        {
            _query = FilterOrdersByDateRange(DateFrom, DateTo);
            Data.TotalSales = await GetTotalSalesAsync(_query);
            Data.TotalOrders = await GetTotalOrdersAsync(_query);
            Data.AverageOrderValue = await GetAverageOrderValueAsync(_query);
            Data.SalesGrowthRate = await GetSalesGrowthRateAsync(_query);
            Data.AverageOrderProcessingTime = await GetAverageOrderProcessingTimeAsync(_query);
            Data.TopSellingProducts = await GetTopSellingProductsAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching sales data.");
            RedirectToPage("/Error");
        }
    }

    private IQueryable<Order> FilterOrdersByDateRange(DateTime dateFrom, DateTime dateTo)
    {
        IQueryable<Order> query = _context.Orders;

        if (DateFrom != default)
        {
            query = query.Where(o => o.OrderDate >= dateFrom);
        }
        if (DateTo != default)
        {
            query = query.Where(o => o.OrderDate <= dateTo);
        }

        return query;
    }

    private async Task<decimal> GetTotalSalesAsync(IQueryable<Order> inputQuery)
    {
        return await inputQuery.Where(o => o.Status == OrderStatus.Completed).SumAsync(o => o.TotalPrice);
    }

    private async Task<int> GetTotalOrdersAsync(IQueryable<Order> inputQuery)
    {
        return await inputQuery.CountAsync();
    }

    private async Task<decimal> GetAverageOrderValueAsync(IQueryable<Order> inputQuery)
    {
        return await inputQuery.AverageAsync(o => o.TotalPrice);
    }

    private async Task<double> GetSalesGrowthRateAsync(IQueryable<Order> inputQuery)
    {
        IQueryable<Order> query = inputQuery;
        var previousPeriodEnd = DateFrom.AddDays(-1);
        var previousPeriodStart = previousPeriodEnd.AddDays(-(DateTo - DateFrom).TotalDays);
        var previousPeriodSales = await _context.Orders
            .Where(o => o.Status == OrderStatus.Completed && o.OrderDate >= previousPeriodStart && o.OrderDate <= previousPeriodEnd)
            .SumAsync(o => o.TotalPrice);

        var currentPeriodSales = await GetTotalSalesAsync(query);

        if (previousPeriodSales == 0) return 100;
        var salesGrowth = ((currentPeriodSales - previousPeriodSales) / previousPeriodSales) * 100;
        return (double)salesGrowth;
    }

    private async Task<double> GetAverageOrderProcessingTimeAsync(IQueryable<Order> inputQuery)
    {
        IQueryable<Order> query = inputQuery.Where(o => o.Status == OrderStatus.Completed);
        return await query.AverageAsync(o => EF.Functions.DateDiffDay(o.OrderDate, o.CompletionDate ?? o.OrderDate));
    }

    private async Task<List<TopProduct>> GetTopSellingProductsAsync()
    {
        return await _context.OrderItems
            .Include(oi => oi.Order)
            .Where(oi => oi.Order!.Status == OrderStatus.Completed && oi.Order.OrderDate >= DateFrom && oi.Order.OrderDate <= DateTo)
            .GroupBy(oi => new { oi.ProductId, oi.ProductItem!.Name })
            .Select(g => new TopProduct
            {
                ProductName = g.Key.Name!,
                Sales = g.Sum(oi => oi.Quantity * oi.UnitPrice)
            })
            .OrderByDescending(tp => tp.Sales)
            .Take(5)
            .ToListAsync();
    }

    public class SalesData
    {
        public decimal TotalSales { get; set; } = 0;
        public int TotalOrders { get; set; }
        public decimal AverageOrderValue { get; set; } = 0;
        public double SalesGrowthRate { get; set; } = 0;
        public double AverageOrderProcessingTime { get; set; } = 0;
        public List<TopProduct> TopSellingProducts { get; set; } = [];
    }

    public class TopProduct
    {
        public string? ProductName { get; set; }
        public decimal Sales { get; set; } = 0;
    }
}

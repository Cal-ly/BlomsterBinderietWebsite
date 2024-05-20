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

    public SalesData Data { get; set; } = new SalesData();
    public string Period { get; set; } = "Month";

    public async Task OnGetAsync(string period = "Month")
    {
        Period = period ?? "Month";

        try
        {
            Data.TotalSales = await GetTotalSalesAsync(Period);
            Data.TotalOrders = await GetTotalOrdersAsync(Period);
            Data.AverageOrderValue = await GetAverageOrderValueAsync(Period);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching sales data.");
            RedirectToPage("/Error");
        }
    }

    private IQueryable<Order> FilterOrdersByPeriod(IQueryable<Order> query, string period)
    {
        var now = DateTime.UtcNow;

        return period switch
        {
            "Day" => query.Where(o => o.OrderDate.Date == now.Date),
            "Month" => query.Where(o => o.OrderDate.Month == now.Month && o.OrderDate.Year == now.Year),
            "Year" => query.Where(o => o.OrderDate.Year == now.Year),
            _ => query
        };
    }

    private async Task<decimal> GetTotalSalesAsync(string period)
    {
        var query = _context.Orders.Where(o => o.Status == OrderStatus.Completed);
        query = FilterOrdersByPeriod(query, period);
        return await query.SumAsync(o => o.GetTotalPrice());
    }

    private async Task<int> GetTotalOrdersAsync(string period)
    {
        IQueryable<Order> query = _context.Orders;
        query = FilterOrdersByPeriod(query, period);
        return await query.CountAsync();
    }

    private async Task<decimal> GetAverageOrderValueAsync(string period)
    {
        IQueryable<Order> query = _context.Orders;
        query = FilterOrdersByPeriod(query, period);
        return await query.AverageAsync(o => o.GetTotalPrice());
    }

    public class SalesData
    {
        public decimal TotalSales { get; set; }
        public int TotalOrders { get; set; }
        public decimal AverageOrderValue { get; set; }
    }
}

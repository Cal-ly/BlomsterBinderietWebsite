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

    [BindProperty]
    public ProductsOrdersData Data { get; set; } = new ProductsOrdersData();

    [BindProperty(SupportsGet = true)]
    public DateTime DateFrom { get; set; } = DateTime.Now.AddDays(-90);

    [BindProperty(SupportsGet = true)]
    public DateTime DateTo { get; set; } = DateTime.Now;

    public async Task OnGetAsync()
    {
        try
        {
            var query = FilterOrdersByDateRange(DateFrom, DateTo);
            Data.TopProducts = await GetTopProductsAsync();
            Data.OrderStatusBreakdown = await GetOrderStatusBreakdownAsync(query);
            Data.AvgTimeToFulfillOrders = await GetAverageTimeToFulfillOrdersAsync(query);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching product and order data.");
            RedirectToPage("/Error");
        }
    }

    private IQueryable<Order> FilterOrdersByDateRange(DateTime dateFrom, DateTime dateTo)
    {
        return _context.Orders.Where(o => o.OrderDate >= dateFrom && o.OrderDate <= dateTo).OrderBy(o => o.OrderDate);
    }

    private async Task<List<TopProduct>> GetTopProductsAsync()
    {
        IQueryable<OrderItem> query = _context.OrderItems
            .Include(oi => oi.Order)
            .Where(oi => oi.Order!.Status == OrderStatus.Completed && oi.Order.OrderDate >= DateFrom && oi.Order.OrderDate <= DateTo);

        return await query
            .GroupBy(oi => new { oi.ProductId, oi.ProductItem!.Name })
            .Select(g => new TopProduct
            {
                ProductName = g.Key.Name ?? "Unknown",
                Sales = g.Sum(oi => oi.Quantity * oi.UnitPrice)
            })
            .OrderByDescending(tp => tp.Sales)
            .Take(10)
            .ToListAsync();
    }

    private async Task<List<OrderStatusBreakdown>> GetOrderStatusBreakdownAsync(IQueryable<Order> inputQuery)
    {
        return await inputQuery.GroupBy(o => o.Status)
            .Select(g => new OrderStatusBreakdown
            {
                Status = g.Key.ToString() ?? "Unknown",
                Count = g.Count()
            })
            .ToListAsync();
    }

    private async Task<double> GetAverageTimeToFulfillOrdersAsync(IQueryable<Order> inputQuery)
    {
        return await inputQuery.Where(o => o.Status == OrderStatus.Completed)
            .AverageAsync(o => EF.Functions.DateDiffDay(o.OrderDate, o.CompletionDate ?? o.OrderDate));
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

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

    public async Task OnGetAsync(DateTime? dateFrom, DateTime? dateTo)
    {
        DateFrom = dateFrom ?? DateTime.Now.AddDays(-90);
        DateTo = dateTo ?? DateTime.Now;

        try
        {
            var orderQuery = FilterOrdersByDateRange(DateFrom, DateTo);
            Data.TopProducts = await GetTopProductsAsync(orderQuery);
            Data.OrderStatusBreakdown = await GetOrderStatusBreakdownAsync(orderQuery);
            Data.AvgTimeToFulfillOrders = await GetAverageTimeToFulfillOrdersAsync(orderQuery);
            Data.AvgItemsPerOrder = GetAverageItemsPerOrder(orderQuery);
            Data.AvgUniqueItemsPerOrder = GetAverageUniqueItemsPerOrder(orderQuery);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching product and order data.");
            RedirectToPage("/Error");
        }
    }

    private IQueryable<Order> FilterOrdersByDateRange(DateTime dateFrom, DateTime dateTo)
    {
        return _context.Orders.Where(o => o.OrderDate >= dateFrom && o.OrderDate <= dateTo).Include(o => o.OrderItems);
    }

    private async Task<List<TopProduct>> GetTopProductsAsync(IQueryable<Order> orderQuery)
    {
        var query = _context.OrderItems
            .Include(oi => oi.Order)
            .Where(oi => oi.Order!.Status == OrderStatus.Completed && orderQuery.Contains(oi.Order));

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

    private async Task<List<OrderStatusBreakdown>> GetOrderStatusBreakdownAsync(IQueryable<Order> orderQuery)
    {
        return await orderQuery
            .GroupBy(o => o.Status)
            .Select(g => new OrderStatusBreakdown
            {
                Status = g.Key.ToString() ?? "Unknown",
                Count = g.Count()
            })
            .ToListAsync();
    }

    private async Task<double> GetAverageTimeToFulfillOrdersAsync(IQueryable<Order> orderQuery)
    {
        IQueryable<Order> query = orderQuery;
        return await query
            .Where(o => o.Status == OrderStatus.Completed)
            .AverageAsync(o => EF.Functions.DateDiffDay(o.OrderDate, o.CompletionDate ?? o.OrderDate));
    }

    private double GetAverageItemsPerOrder(IQueryable<Order> orderQuery)
    {
        List<Order> orders = [.. orderQuery.Where(o => o.Status == OrderStatus.Completed).Include(o => o.OrderItems)];
        return orders.Average(o => o.OrderItems.Sum(oi => oi.Quantity));
    }

    private double GetAverageUniqueItemsPerOrder(IQueryable<Order> orderQuery)
    {
        List<Order> orders = [.. orderQuery.Where(o => o.Status == OrderStatus.Completed).Include(o => o.OrderItems)];

        return (double)orders.Average(o => o.OrderItems.Count);
    }

    public class ProductsOrdersData
    {
        public List<TopProduct> TopProducts { get; set; } = new List<TopProduct>();
        public List<OrderStatusBreakdown> OrderStatusBreakdown { get; set; } = new List<OrderStatusBreakdown>();
        public double AvgTimeToFulfillOrders { get; set; }
        public double AvgItemsPerOrder { get; set; }
        public double AvgUniqueItemsPerOrder { get; set; }
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

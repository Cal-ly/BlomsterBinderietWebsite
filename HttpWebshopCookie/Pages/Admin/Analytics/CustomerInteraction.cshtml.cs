namespace HttpWebshopCookie.Pages.Admin.Analytics;

[Authorize(Policy = "managerAccess")]
public class CustomerInteractionModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<CustomerInteractionModel> _logger;

    public CustomerInteractionModel(ApplicationDbContext context, ILogger<CustomerInteractionModel> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public CustomerInteractionData Data { get; set; } = new CustomerInteractionData();
    public DateTime? DateFrom { get; set; } = DateTime.UtcNow.AddDays(-90);
    public DateTime? DateTo { get; set; } = DateTime.UtcNow;

    public async Task OnGetAsync(DateTime? dateFrom, DateTime? dateTo)
    {
        DateFrom = dateFrom ?? DateTime.UtcNow.AddDays(-90);
        DateTo = dateTo ?? DateTime.UtcNow;

        try
        {
            await FetchCustomerInteractionDataAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching customer interaction data.");
            RedirectToPage("/Error");
        }
    }

    private async Task FetchCustomerInteractionDataAsync()
    {
        var tasks = new Task[]
        {
            Task.Run(async () => Data.CustomerGrowth = await GetCustomerGrowthAsync()),
            Task.Run(async () => Data.BasketActivitySummary = await GetBasketActivitySummaryAsync()),
            Task.Run(async () => Data.AvgTimeSpentOnSite = await GetAvgTimeSpentOnSiteAsync()),
            Task.Run(async () => Data.AvgMaxItemsInBasket = await GetAvgMaxItemsInBasketAsync()),
            Task.Run(async () => Data.AvgActivitiesPerSession = await GetAverageActivitiesPerSessionAsync()),
            Task.Run(async () => Data.MostAddedProducts = await GetMostAddedProductsAsync()),
            Task.Run(async () => Data.MostRemovedProducts = await GetMostRemovedProductsAsync()),
            Task.Run(async () => Data.ActivityTypeCount = await GetActivityTypeCountAsync())
        };

        await Task.WhenAll(tasks);
    }

    private IQueryable<Customer> FilterCustomersByDateRange(IQueryable<Customer> query)
    {
        return query.Where(c => c.EnrollmentDate.HasValue && c.EnrollmentDate.Value >= DateFrom!.Value && c.EnrollmentDate.Value <= DateTo!.Value);
    }

    private async Task<List<CustomerGrowth>> GetCustomerGrowthAsync()
    {
        var query = _context.Users.OfType<Customer>();
        query = FilterCustomersByDateRange(query);

        var customerGrowth = await query.GroupBy(c => c.EnrollmentDate!.Value.Date.ToString("yyyy-MM-dd"))
                                        .Select(g => new CustomerGrowth
                                        {
                                            Period = g.Key,
                                            NewCustomers = g.Count()
                                        })
                                        .ToListAsync();

        return customerGrowth;
    }

    private IQueryable<BasketActivity> FilterBasketActivitiesByDateRange(IQueryable<BasketActivity> query)
    {
        return query.Where(ba => ba.Timestamp.HasValue && ba.Timestamp.Value >= DateFrom!.Value && ba.Timestamp.Value <= DateTo!.Value);
    }

    private async Task<List<BasketActivitySummary>> GetBasketActivitySummaryAsync()
    {
        IQueryable<BasketActivity> query = _context.BasketActivities;
        query = FilterBasketActivitiesByDateRange(query);

        var basketActivitySummary = await query.GroupBy(ba => ba.ActivityType)
                                               .Select(g => new BasketActivitySummary
                                               {
                                                   ActivityType = g.Key ?? "Unknown",
                                                   Count = g.Count(),
                                                   TotalQuantityChanged = g.Sum(ba => ba.QuantityChanged ?? 0)
                                               })
                                               .ToListAsync();

        return basketActivitySummary;
    }

    private async Task<double> GetAvgTimeSpentOnSiteAsync()
    {
        var query = FilterBasketActivitiesByDateRange(_context.BasketActivities);

        var sessionDurations = await query.Where(ba => !string.IsNullOrEmpty(ba.SessionId))
                                          .GroupBy(ba => ba.SessionId)
                                          .Select(g => EF.Functions.DateDiffMinute(g.Min(ba => ba.Timestamp), g.Max(ba => ba.Timestamp)))
                                          .ToListAsync();
        var decimalAvg = Math.Round((decimal)sessionDurations.Average()!, 2);
        return (double)decimalAvg;
    }

    private async Task<double> GetAvgMaxItemsInBasketAsync()
    {
        var query = FilterBasketActivitiesByDateRange(_context.BasketActivities);

        var maxItems = await query.GroupBy(ba => ba.SessionId)
                                  .Select(g => g.Max(ba => ba.QuantityChanged))
                                  .ToListAsync();

        return Math.Round(maxItems.Average() ?? 0, 2);
    }

    private async Task<double> GetAverageActivitiesPerSessionAsync()
    {
        var query = FilterBasketActivitiesByDateRange(_context.BasketActivities);

        var activities = await query.GroupBy(ba => ba.SessionId)
                                    .Select(g => g.Count())
                                    .ToListAsync();

        return Math.Round((double)activities.Average(), 2);
    }

    private async Task<List<ProductActivity>> GetMostAddedProductsAsync()
    {
        var query = FilterBasketActivitiesByDateRange(_context.BasketActivities);

        return await query.Where(ba => ba.ActivityType == "Add")
                          .GroupBy(ba => new { ba.ProductId, ba.Product!.Name })
                          .Select(g => new ProductActivity { ProductName = g.Key.Name!, Count = g.Count() })
                          .OrderByDescending(pa => pa.Count)
                          .Take(5)
                          .ToListAsync();
    }

    private async Task<List<ProductActivity>> GetMostRemovedProductsAsync()
    {
        var query = FilterBasketActivitiesByDateRange(_context.BasketActivities);

        return await query.Where(ba => ba.ActivityType == "Remove")
                          .GroupBy(ba => new { ba.ProductId, ba.Product!.Name })
                          .Select(g => new ProductActivity { ProductName = g.Key.Name!, Count = g.Count() })
                          .OrderByDescending(pa => pa.Count)
                          .Take(5)
                          .ToListAsync();
    }

    private async Task<Dictionary<string, int>> GetActivityTypeCountAsync()
    {
        var query = FilterBasketActivitiesByDateRange(_context.BasketActivities);

        return await query.GroupBy(ba => ba.ActivityType)
                          .Select(g => new { ActivityType = g.Key!, Count = g.Count() })
                          .ToDictionaryAsync(x => x.ActivityType, x => x.Count);
    }

    public class CustomerInteractionData
    {
        public List<CustomerGrowth> CustomerGrowth { get; set; } = new List<CustomerGrowth>();
        public List<BasketActivitySummary> BasketActivitySummary { get; set; } = new List<BasketActivitySummary>();
        public double AvgTimeSpentOnSite { get; set; }
        public double AvgMaxItemsInBasket { get; set; }
        public double AvgActivitiesPerSession { get; set; }
        public List<ProductActivity> MostAddedProducts { get; set; } = new List<ProductActivity>();
        public List<ProductActivity> MostRemovedProducts { get; set; } = new List<ProductActivity>();
        public Dictionary<string, int> ActivityTypeCount { get; set; } = new Dictionary<string, int>();
    }

    public class CustomerGrowth
    {
        public string Period { get; set; } = string.Empty;
        public int NewCustomers { get; set; } = 0;
    }

    public class BasketActivitySummary
    {
        public string ActivityType { get; set; } = string.Empty;
        public int Count { get; set; }
        public int TotalQuantityChanged { get; set; }
    }

    public class ProductActivity
    {
        public string ProductName { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}

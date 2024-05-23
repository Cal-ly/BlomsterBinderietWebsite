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

    [BindProperty]
    public CustomerInteractionData Data { get; set; } = new CustomerInteractionData();

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
            var customerQuery = FilterCustomersByDateRange(DateFrom, DateTo);
            var basketQuery = FilterBasketActivitiesByDateRange(DateFrom, DateTo);
            Data.CustomerGrowth = await GetCustomerGrowthAsync(customerQuery);
            Data.BasketActivitySummary = await GetBasketActivitySummaryAsync(basketQuery);
            Data.AvgTimeSpentOnSite = await GetAvgTimeSpentOnSiteAsync(basketQuery);
            Data.AvgMaxItemsInBasket = await GetAvgMaxItemsInBasketAsync(basketQuery);
            Data.AvgActivitiesPerSession = await GetAverageActivitiesPerSessionAsync(basketQuery);
            Data.MostAddedProducts = await GetMostAddedProductsAsync(basketQuery);
            Data.MostRemovedProducts = await GetMostRemovedProductsAsync(basketQuery);
            Data.ActivityTypeCount = await GetActivityTypeCountAsync(basketQuery);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching customer interaction data.");
            RedirectToPage("/Error");
        }
    }

    private IQueryable<Customer> FilterCustomersByDateRange(DateTime dateFrom, DateTime dateTo)
    {

        return _context.Customers.Where(c => c.EnrollmentDate >= dateFrom && c.EnrollmentDate <= dateTo);
    }

    private async Task<List<CustomerGrowth>> GetCustomerGrowthAsync(IQueryable<Customer> customerQuery)
    {
        IQueryable<Customer> query = customerQuery;
        return await query.GroupBy(c => c.EnrollmentDate!.Value.Date.ToString("yyyy-MM-dd"))
                                        .Select(g => new CustomerGrowth
                                        {
                                            Period = g.Key,
                                            NewCustomers = g.Count()
                                        })
                                        .ToListAsync();
    }

    private IQueryable<BasketActivity> FilterBasketActivitiesByDateRange(DateTime dateFrom, DateTime dateTo)
    {
        return _context.BasketActivities.Where(ba => ba.Timestamp >= dateFrom && ba.Timestamp <= dateTo);
    }

    private async Task<List<BasketActivitySummary>> GetBasketActivitySummaryAsync(IQueryable<BasketActivity> basketQuery)
    {
        IQueryable<BasketActivity> query = basketQuery;
        return await query.GroupBy(ba => ba.ActivityType)
                          .Select(g => new BasketActivitySummary
                          {
                              ActivityType = g.Key ?? "Unknown",
                              Count = g.Count(),
                              TotalQuantityChanged = g.Sum(ba => ba.QuantityChanged ?? 0)
                          })
                          .ToListAsync();
    }

    private async Task<double> GetAvgTimeSpentOnSiteAsync(IQueryable<BasketActivity> basketQuery)
    {
        IQueryable<BasketActivity> query = basketQuery;
        var sessionDurations = await query.Where(ba => !string.IsNullOrEmpty(ba.SessionId))
                                          .GroupBy(ba => ba.SessionId)
                                          .Select(g => EF.Functions.DateDiffMinute(g.Min(ba => ba.Timestamp), g.Max(ba => ba.Timestamp)))
                                          .ToListAsync();

        return Math.Round(sessionDurations.Average() ?? 0, 2);
    }

    private async Task<double> GetAvgMaxItemsInBasketAsync(IQueryable<BasketActivity> basketQuery)
    {
        IQueryable<BasketActivity> query = basketQuery;
        var maxItems = await query.GroupBy(ba => ba.SessionId)
                                  .Select(g => g.Max(ba => ba.QuantityChanged))
                                  .ToListAsync();

        return Math.Round(maxItems.Average() ?? 0, 2);
    }

    private async Task<double> GetAverageActivitiesPerSessionAsync(IQueryable<BasketActivity> basketQuery)
    {
        IQueryable<BasketActivity> query = basketQuery;
        var activities = await query.GroupBy(ba => ba.SessionId)
                                    .Select(g => g.Count())
                                    .ToListAsync();

        return Math.Round((double)activities.Average(), 2);
    }

    private async Task<List<ProductActivity>> GetMostAddedProductsAsync(IQueryable<BasketActivity> basketQuery)
    {
        IQueryable<BasketActivity> query = basketQuery;
        return await query.Where(ba => ba.ActivityType == "Add")
                          .GroupBy(ba => new { ba.ProductId, ba.Product!.Name })
                          .Select(g => new ProductActivity { ProductName = g.Key.Name!, Count = g.Count() })
                          .OrderByDescending(pa => pa.Count)
                          .Take(5)
                          .ToListAsync();
    }

    private async Task<List<ProductActivity>> GetMostRemovedProductsAsync(IQueryable<BasketActivity> basketQuery)
    {
        IQueryable<BasketActivity> query = basketQuery;
        return await query.Where(ba => ba.ActivityType == "Remove")
                          .GroupBy(ba => new { ba.ProductId, ba.Product!.Name })
                          .Select(g => new ProductActivity { ProductName = g.Key.Name!, Count = g.Count() })
                          .OrderByDescending(pa => pa.Count)
                          .Take(5)
                          .ToListAsync();
    }

    private async Task<Dictionary<string, int>> GetActivityTypeCountAsync(IQueryable<BasketActivity> basketQuery)
    {
        IQueryable<BasketActivity> query = basketQuery;
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
        public string? Period { get; set; }
        public int NewCustomers { get; set; }
    }

    public class BasketActivitySummary
    {
        public string? ActivityType { get; set; }
        public int Count { get; set; }
        public int TotalQuantityChanged { get; set; }
    }

    public class ProductActivity
    {
        public string? ProductName { get; set; }
        public int Count { get; set; }
    }
}
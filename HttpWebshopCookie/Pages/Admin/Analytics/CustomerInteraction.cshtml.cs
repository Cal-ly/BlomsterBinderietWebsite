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
    public string Period { get; set; } = "Month";
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }

    public async Task OnGetAsync(string period = "Month", DateTime? dateFrom = null, DateTime? dateTo = null)
    {
        Period = period ?? "Month";
        DateFrom = dateFrom;
        DateTo = dateTo;

        try
        {
            Data.CustomerGrowth = await GetCustomerGrowthAsync();
            Data.BasketActivitySummary = await GetBasketActivitySummaryAsync();
            Data.AvgTimeSpentOnSite = await GetAvgTimeSpentOnSiteAsync();
            Data.AvgMaxItemsInBasket = await GetAvgMaxItemsInBasketAsync();
            Data.AvgActivitiesPerSession = await GetAverageActivitiesPerSessionAsync();
            Data.MostAddedProducts = await GetMostAddedProductsAsync();
            Data.MostRemovedProducts = await GetMostRemovedProductsAsync();
            Data.ActivityTypeCount = await GetActivityTypeCountAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching customer interaction data.");
            RedirectToPage("/Error");
        }
    }

    private IQueryable<Customer> FilterCustomersByPeriod(IQueryable<Customer> query)
    {
        if (DateFrom.HasValue && DateTo.HasValue)
        {
            return query.Where(c => c.EnrollmentDate.HasValue && c.EnrollmentDate.Value >= DateFrom.Value && c.EnrollmentDate.Value <= DateTo.Value);
        }
        else
        {
            var now = DateTime.UtcNow;

            return Period switch
            {
                "Day" => query.Where(c => c.EnrollmentDate.HasValue && c.EnrollmentDate.Value.Date == now.Date),
                "Month" => query.Where(c => c.EnrollmentDate.HasValue && c.EnrollmentDate.Value.Year == now.Year && c.EnrollmentDate.Value.Month == now.Month),
                "Year" => query.Where(c => c.EnrollmentDate.HasValue && c.EnrollmentDate.Value.Year == now.Year),
                _ => query
            };
        }
    }

    private async Task<List<CustomerGrowth>> GetCustomerGrowthAsync()
    {
        var query = _context.Users.OfType<Customer>();
        query = FilterCustomersByPeriod(query);

        var customerGrowth = await query.GroupBy(c =>
            DateFrom.HasValue && DateTo.HasValue ? c.EnrollmentDate!.Value.Date.ToString("yyyy-MM-dd") :
            Period == "Day" ? c.EnrollmentDate!.Value.Date.ToString("yyyy-MM-dd") :
            Period == "Month" ? $"{c.EnrollmentDate!.Value.Year}-{c.EnrollmentDate.Value.Month:D2}" :
            Period == "Year" ? c.EnrollmentDate!.Value.Year.ToString() :
            string.Empty
        )
        .Select(g => new CustomerGrowth
        {
            Period = g.Key,
            NewCustomers = g.Count()
        })
        .ToListAsync();

        return customerGrowth;
    }

    private IQueryable<BasketActivity> FilterBasketActivitiesByPeriod(IQueryable<BasketActivity> query)
    {
        if (DateFrom.HasValue && DateTo.HasValue)
        {
            return query.Where(ba => ba.Timestamp.HasValue && ba.Timestamp.Value >= DateFrom.Value && ba.Timestamp.Value <= DateTo.Value);
        }
        else
        {
            var now = DateTime.UtcNow;

            return Period switch
            {
                "Day" => query.Where(ba => ba.Timestamp.HasValue && ba.Timestamp.Value.Date == now.Date),
                "Month" => query.Where(ba => ba.Timestamp.HasValue && ba.Timestamp.Value.Year == now.Year && ba.Timestamp.Value.Month == now.Month),
                "Year" => query.Where(ba => ba.Timestamp.HasValue && ba.Timestamp.Value.Year == now.Year),
                _ => query
            };
        }
    }

    private async Task<List<BasketActivitySummary>> GetBasketActivitySummaryAsync()
    {
        IQueryable<BasketActivity> query = _context.BasketActivities;
        query = FilterBasketActivitiesByPeriod(query);

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
        var query = FilterBasketActivitiesByPeriod(_context.BasketActivities);

        var sessionDurations = await query
            .Where(ba => !string.IsNullOrEmpty(ba.SessionId))
            .GroupBy(ba => ba.SessionId)
            .Select(g => EF.Functions.DateDiffMinute(g.Min(ba => ba.Timestamp), g.Max(ba => ba.Timestamp)))
            .ToListAsync();

        return Math.Round(sessionDurations.Average() ?? 0, 2);
    }

    private async Task<double> GetAvgMaxItemsInBasketAsync()
    {
        var query = FilterBasketActivitiesByPeriod(_context.BasketActivities);

        var maxItems = await query
            .GroupBy(ba => ba.SessionId)
            .Select(g => g.Max(ba => ba.QuantityChanged))
            .ToListAsync();

        return Math.Round(maxItems.Average() ?? 0, 2);
    }

    private async Task<double> GetAverageActivitiesPerSessionAsync()
    {
        var query = FilterBasketActivitiesByPeriod(_context.BasketActivities);

        var activities = await query
            .GroupBy(ba => ba.SessionId)
            .Select(g => g.Count())
            .ToListAsync();

        return Math.Round((double)activities.Average(), 2);
    }

    private async Task<List<ProductActivity>> GetMostAddedProductsAsync() //TODO make generic for all activity types
    {
        var query = FilterBasketActivitiesByPeriod(_context.BasketActivities);

        return await query
            .Where(ba => ba.ActivityType == "Add")
            .GroupBy(ba => new { ba.ProductId, ba.Product!.Name })
            .Select(g => new ProductActivity { ProductName = g.Key.Name!, Count = g.Count() })
            .OrderByDescending(pa => pa.Count)
            .Take(5)
            .ToListAsync();
    }

    private async Task<List<ProductActivity>> GetMostRemovedProductsAsync()
    {
        var query = FilterBasketActivitiesByPeriod(_context.BasketActivities);

        return await query
            .Where(ba => ba.ActivityType == "Remove")
            .GroupBy(ba => new { ba.ProductId, ba.Product!.Name })
            .Select(g => new ProductActivity { ProductName = g.Key.Name!, Count = g.Count() })
            .OrderByDescending(pa => pa.Count)
            .Take(5)
            .ToListAsync();
    }

    private async Task<Dictionary<string, int>> GetActivityTypeCountAsync()
    {
        var query = FilterBasketActivitiesByPeriod(_context.BasketActivities);

        return await query
            .GroupBy(ba => ba.ActivityType)
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

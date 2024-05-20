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

    public async Task OnGetAsync(string period = "Month")
    {
        Period = period ?? "Month";

        try
        {
            Data.CustomerGrowth = await GetCustomerGrowthAsync(Period);
            Data.BasketActivitySummary = await GetBasketActivitySummaryAsync(Period);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching customer interaction data.");
            // Consider redirecting to an error page or setting a flag to display an error message in the view
        }
    }

    private IQueryable<Customer> FilterCustomersByPeriod(IQueryable<Customer> query, string period)
    {
        var now = DateTime.UtcNow;

        return period switch
        {
            "Day" => query.Where(c => c.EnrollmentDate.HasValue && c.EnrollmentDate.Value.Date == now.Date),
            "Month" => query.Where(c => c.EnrollmentDate.HasValue && c.EnrollmentDate.Value.Year == now.Year && c.EnrollmentDate.Value.Month == now.Month),
            "Year" => query.Where(c => c.EnrollmentDate.HasValue && c.EnrollmentDate.Value.Year == now.Year),
            _ => query
        };
    }

    private async Task<List<CustomerGrowth>> GetCustomerGrowthAsync(string period)
    {
        var query = _context.Users.OfType<Customer>();
        query = FilterCustomersByPeriod(query, period);

        var customerGrowth = await query.GroupBy(c =>
            period == "Day" ? c.EnrollmentDate!.Value.Date.ToString("yyyy-MM-dd") :
            period == "Month" ? $"{c.EnrollmentDate!.Value.Year}-{c.EnrollmentDate.Value.Month:D2}" :
            period == "Year" ? c.EnrollmentDate!.Value.Year.ToString() :
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

    private IQueryable<BasketActivity> FilterBasketActivitiesByPeriod(IQueryable<BasketActivity> query, string period)
    {
        var now = DateTime.UtcNow;

        return period switch
        {
            "Day" => query.Where(ba => ba.Timestamp.HasValue && ba.Timestamp.Value.Date == now.Date),
            "Month" => query.Where(ba => ba.Timestamp.HasValue && ba.Timestamp.Value.Year == now.Year && ba.Timestamp.Value.Month == now.Month),
            "Year" => query.Where(ba => ba.Timestamp.HasValue && ba.Timestamp.Value.Year == now.Year),
            _ => query
        };
    }

    private async Task<List<BasketActivitySummary>> GetBasketActivitySummaryAsync(string period)
    {
        IQueryable<BasketActivity> query = _context.BasketActivities;
        query = FilterBasketActivitiesByPeriod(query, period);

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

    public class CustomerInteractionData
    {
        public List<CustomerGrowth> CustomerGrowth { get; set; } = new List<CustomerGrowth>();
        public List<BasketActivitySummary> BasketActivitySummary { get; set; } = new List<BasketActivitySummary>();
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
}
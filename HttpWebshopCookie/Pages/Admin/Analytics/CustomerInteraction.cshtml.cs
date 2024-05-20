namespace HttpWebshopCookie.Pages.Admin.Analytics;

[Authorize(Policy = "managerAccess")]
public class CustomerInteractionModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public CustomerInteractionModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public CustomerInteractionData Data { get; set; }
    public string Period { get; set; }

    public async Task OnGetAsync(string period = "Month")
    {
        Period = period;
        Data = new CustomerInteractionData
        {
            CustomerGrowth = await GetCustomerGrowthAsync(period),
            BasketActivitySummary = await GetBasketActivitySummaryAsync(period)
        };
    }

    private async Task<List<CustomerGrowth>> GetCustomerGrowthAsync(string period)
    {
        IQueryable<Customer> query = _context.Users.OfType<Customer>();

        if (period == "Day")
        {
            return await query.Where(c => c.EnrollmentDate.HasValue)
                              .GroupBy(c => c.EnrollmentDate.Value.Date)
                              .Select(g => new CustomerGrowth { Period = g.Key.ToString("yyyy-MM-dd"), NewCustomers = g.Count() })
                              .ToListAsync();
        }
        else if (period == "Month")
        {
            return await query.Where(c => c.EnrollmentDate.HasValue)
                              .GroupBy(c => new { c.EnrollmentDate.Value.Year, c.EnrollmentDate.Value.Month })
                              .Select(g => new CustomerGrowth { Period = $"{g.Key.Year}-{g.Key.Month:D2}", NewCustomers = g.Count() })
                              .ToListAsync();
        }
        else if (period == "Year")
        {
            return await query.Where(c => c.EnrollmentDate.HasValue)
                              .GroupBy(c => c.EnrollmentDate.Value.Year)
                              .Select(g => new CustomerGrowth { Period = g.Key.ToString(), NewCustomers = g.Count() })
                              .ToListAsync();
        }
        else
        {
            return new List<CustomerGrowth>();
        }
    }

    private async Task<List<BasketActivitySummary>> GetBasketActivitySummaryAsync(string period)
    {
        IQueryable<BasketActivity> query = _context.BasketActivities;

        switch (period)
        {
            case "Day":
                query = query.Where(ba => ba.Timestamp.Value.Date == DateTime.UtcNow.Date);
                break;
            case "Month":
                query = query.Where(ba => ba.Timestamp.Value.Month == DateTime.UtcNow.Month && ba.Timestamp.Value.Year == DateTime.UtcNow.Year);
                break;
            case "Year":
                query = query.Where(ba => ba.Timestamp.Value.Year == DateTime.UtcNow.Year);
                break;
        }

        return await query.GroupBy(ba => ba.ActivityType)
                          .Select(g => new BasketActivitySummary
                          {
                              ActivityType = g.Key,
                              Count = g.Count(),
                              TotalQuantityChanged = g.Sum(ba => ba.QuantityChanged ?? 0)
                          })
                          .ToListAsync();
    }

    public class CustomerInteractionData
    {
        public List<CustomerGrowth> CustomerGrowth { get; set; }
        public List<BasketActivitySummary> BasketActivitySummary { get; set; }
    }

    public class CustomerGrowth
    {
        public string Period { get; set; }
        public int NewCustomers { get; set; }
    }

    public class BasketActivitySummary
    {
        public string ActivityType { get; set; }
        public int Count { get; set; }
        public int TotalQuantityChanged { get; set; }
    }
}

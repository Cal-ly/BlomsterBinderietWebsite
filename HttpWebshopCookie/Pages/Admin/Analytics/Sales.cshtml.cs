namespace HttpWebshopCookie.Pages.Admin.Analytics;

[Authorize(Policy = "managerAccess")]
public class SalesModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public SalesModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public SalesData Data { get; set; }
    public string Period { get; set; }

    public async Task OnGetAsync(string period = "Month")
    {
        Period = period;
        Data = new SalesData
        {
            TotalSales = await GetTotalSalesAsync(period),
            TotalOrders = await GetTotalOrdersAsync(period),
            AverageOrderValue = await GetAverageOrderValueAsync(period)
        };
    }

    private async Task<decimal> GetTotalSalesAsync(string period)
    {
        var query = _context.Orders.Where(o => o.Status == OrderStatus.Completed);
        switch (period)
        {
            case "Day":
                return await query.Where(o => o.OrderDate.Date == DateTime.UtcNow.Date).SumAsync(o => o.GetTotalPrice());
            case "Month":
                return await query.Where(o => o.OrderDate.Month == DateTime.UtcNow.Month && o.OrderDate.Year == DateTime.UtcNow.Year).SumAsync(o => o.GetTotalPrice());
            case "Year":
                return await query.Where(o => o.OrderDate.Year == DateTime.UtcNow.Year).SumAsync(o => o.GetTotalPrice());
            default:
                return await query.SumAsync(o => o.GetTotalPrice());
        }
    }

    private async Task<int> GetTotalOrdersAsync(string period)
    {
        var query = _context.Orders;
        switch (period)
        {
            case "Day":
                return await query.Where(o => o.OrderDate.Date == DateTime.UtcNow.Date).CountAsync();
            case "Month":
                return await query.Where(o => o.OrderDate.Month == DateTime.UtcNow.Month && o.OrderDate.Year == DateTime.UtcNow.Year).CountAsync();
            case "Year":
                return await query.Where(o => o.OrderDate.Year == DateTime.UtcNow.Year).CountAsync();
            default:
                return await query.CountAsync();
        }
    }

    private async Task<decimal> GetAverageOrderValueAsync(string period)
    {
        var query = _context.Orders;
        switch (period)
        {
            case "Day":
                return await query.Where(o => o.OrderDate.Date == DateTime.UtcNow.Date).AverageAsync(o => o.GetTotalPrice());
            case "Month":
                return await query.Where(o => o.OrderDate.Month == DateTime.UtcNow.Month && o.OrderDate.Year == DateTime.UtcNow.Year).AverageAsync(o => o.GetTotalPrice());
            case "Year":
                return await query.Where(o => o.OrderDate.Year == DateTime.UtcNow.Year).AverageAsync(o => o.GetTotalPrice());
            default:
                return await query.AverageAsync(o => o.GetTotalPrice());
        }
    }

    public class SalesData
    {
        public decimal TotalSales { get; set; }
        public int TotalOrders { get; set; }
        public decimal AverageOrderValue { get; set; }
    }
}

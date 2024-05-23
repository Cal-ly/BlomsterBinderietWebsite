namespace HttpWebshopCookie.Pages.Orders;

public class OrderListingsModel(ApplicationDbContext context) : PageModel
{
    public List<Order> Orders { get; set; } = new List<Order>();
    public OrderStatus? FilterStatus { get; set; }
    public string? CustomerName { get; set; }
    public string? EmployeeName { get; set; }
    [BindProperty(SupportsGet = true)]
    public DateTime? StartDate { get; set; }
    [BindProperty(SupportsGet = true)]
    public DateTime? EndDate { get; set; }
    [BindProperty(SupportsGet = true)]
    public string SortColumn { get; set; } = "OrderDate";
    [BindProperty(SupportsGet = true)]
    public string SortDirection { get; set; } = "desc";
    [BindProperty(SupportsGet = true)]
    public string SoiFilter { get; set; } = "all";

    public async Task OnGetAsync(OrderStatus? filterStatus, string? customerName, string? employeeName, string sortColumn, string sortDirection, string soiFilter)
    {
        FilterStatus = filterStatus;
        CustomerName = customerName;
        EmployeeName = employeeName;
        SortColumn = sortColumn ?? "OrderDate";
        SortDirection = sortDirection ?? "desc";
        SoiFilter = soiFilter ?? "all";

        var ordersQuery = context.Orders
            .Include(o => o.Customer)
            .Include(o => o.Guest)
            .Include(o => o.Employee)
            .Include(o => o.OrderItems)
            .Include(o => o.SpecialOrderInstruction)
            .AsQueryable();

        if (filterStatus.HasValue)
        {
            ordersQuery = ordersQuery.Where(o => o.Status == filterStatus);
        }

        if (!string.IsNullOrEmpty(customerName))
        {
            ordersQuery = ordersQuery.Where(o =>
                (o.Customer != null && (o.Customer.FirstName + " " + o.Customer.LastName).Contains(customerName)) ||
                (o.Guest != null && (o.Guest.FirstName + " " + o.Guest.LastName).Contains(customerName)));
        }

        if (StartDate.HasValue)
        {
            ordersQuery = ordersQuery.Where(o => o.OrderDate >= StartDate.Value);
        }
        if (EndDate.HasValue)
        {
            ordersQuery = ordersQuery.Where(o => o.OrderDate <= EndDate.Value);
        }

        if (!string.IsNullOrEmpty(employeeName))
        {
            ordersQuery = ordersQuery.Where(o => o.Employee != null &&
                                                (o.Employee.FirstName + " " + o.Employee.LastName)
                                                .Contains(employeeName));
        }

        if (SoiFilter == "yes")
        {
            ordersQuery = ordersQuery.Where(o => o.SpecialOrderInstruction != null);
        }
        else if (SoiFilter == "no")
        {
            ordersQuery = ordersQuery.Where(o => o.SpecialOrderInstruction == null);
        }

        ordersQuery = SortColumn switch
        {
            "OrderDate" => SortDirection == "asc" ? ordersQuery.OrderBy(o => o.OrderDate) 
                                                    : ordersQuery.OrderByDescending(o => o.OrderDate),
            "Status" => SortDirection == "asc" ? ordersQuery.OrderBy(o => o.Status) 
                                                    : ordersQuery.OrderByDescending(o => o.Status),
            "Soi" => SortDirection == "asc" ? ordersQuery.OrderBy(o => o.SpecialOrderInstruction != null) 
                                                    : ordersQuery.OrderByDescending(o => o.SpecialOrderInstruction != null),
            "Customer" => SortDirection == "asc" ? ordersQuery.OrderBy(o => (o.Customer != null ? o.Customer.FirstName + " " + o.Customer.LastName : o.Guest!.FirstName + " " + o.Guest.LastName)) 
                                                    : ordersQuery.OrderByDescending(o => o.Customer != null ? o.Customer.FirstName + " " + o.Customer.LastName : o.Guest!.FirstName + " " + o.Guest.LastName),
            "Employee" => SortDirection == "asc" ? ordersQuery.OrderBy(o => o.Employee != null ? o.Employee.FirstName + " " + o.Employee.LastName : "") 
                                                    : ordersQuery.OrderByDescending(o => o.Employee != null ? o.Employee.FirstName + " " + o.Employee.LastName : ""),
            "TotalPrice" => SortDirection == "asc" ? ordersQuery.OrderBy(o => o.TotalPrice) 
                                                    : ordersQuery.OrderByDescending(o => o.TotalPrice),
            _ => ordersQuery.OrderByDescending(o => o.OrderDate)
        };

        Orders = await ordersQuery.ToListAsync();
    }
    public IActionResult OnPostSearch(OrderStatus? filterStatus, string? customerName, string? employeeName, string sortColumn, string sortDirection, string soiFilter)
    {
        return RedirectToPage(new { filterStatus, customerName, employeeName, sortColumn, sortDirection, soiFilter });
    }
}
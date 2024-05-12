namespace HttpWebshopCookie.Pages.Products;

public class IndexAltModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly BasketService _basketService;

    [BindProperty]
    public List<ProductViewModel> ProductList { get; set; } = new List<ProductViewModel>();
    public Dictionary<string, int> ProductQuantities { get; set; } = new Dictionary<string, int>();
    public string? SearchTerm { get; set; }
    public string? SortBy { get; set; }
    public string? SortOrder { get; set; }
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 9;
    public int TotalPages { get; set; }

    public IndexAltModel(ApplicationDbContext context, BasketService basketService)
    {
        _context = context;
        _basketService = basketService;
    }

    public async Task OnGetAsync(string searchTerm, int pageIndex, int pageSize, string sortBy, string sortOrder)
    {
        AssignPageProperties(searchTerm, pageIndex, pageSize, sortBy, sortOrder);

        var query = BuildProductQuery();
        TotalPages = await CalculateTotalPages(query);
        ProductList = await FetchProducts(query);
        ProductQuantities = await FetchProductQuantities();
    }

    private void AssignPageProperties(string searchTerm, int pageIndex, int pageSize, string sortBy, string sortOrder)
    {
        SearchTerm = searchTerm ?? string.Empty;
        PageIndex = pageIndex > 0 ? pageIndex : 1;
        PageSize = pageSize > 0 ? pageSize : 9;
        SortBy = !string.IsNullOrEmpty(sortBy) ? sortBy : "Name";
        SortOrder = !string.IsNullOrEmpty(sortOrder) ? sortOrder : "Ascending";
    }

    private IQueryable<Product> BuildProductQuery()
    {
        var query = _context.Products
            .Include(p => p.ProductTags).ThenInclude(pt => pt.Tag)
            .AsQueryable()
            .AsNoTracking();

        if (!string.IsNullOrEmpty(SearchTerm))
        {
            query = query.Where(p => p.ProductTags.Any(pt =>
                EF.Functions.Like(pt.Tag!.Occasion, $"%{SearchTerm}%") ||
                EF.Functions.Like(pt.Tag!.Category, $"%{SearchTerm}%") ||
                EF.Functions.Like(pt.Tag!.SubCategory, $"%{SearchTerm}%")));
        }

        query = SortProducts(query);
        return query;
    }

    private IQueryable<Product> SortProducts(IQueryable<Product> query)
    {
        return SortBy switch
        {
            "Price" when SortOrder == "Descending" => query.OrderByDescending(p => p.Price),
            "Price" => query.OrderBy(p => p.Price),
            "Name" when SortOrder == "Descending" => query.OrderByDescending(p => p.Name),
            _ => query.OrderBy(p => p.Name),
        };
    }

    private async Task<int> CalculateTotalPages(IQueryable<Product> query)
    {
        int totalItems = await query.CountAsync();
        return (int)Math.Ceiling(totalItems / (double)PageSize);
    }

    private async Task<List<ProductViewModel>> FetchProducts(IQueryable<Product> query)
    {
        return await query
            .Skip((PageIndex - 1) * PageSize)
            .Take(PageSize)
            .Select(p => new ProductViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price.ToString("C"),
                Description = p.Description,
                Tags = p.ProductTags.Select(pt => new TagViewModel
                {
                    Occasion = pt.Tag!.Occasion,
                    Category = pt.Tag!.Category,
                    SubCategory = pt.Tag!.SubCategory
                })
                .Distinct()
                .OrderBy(t => t.Category)
                .ThenBy(t => t.SubCategory)
                .ToList()
            })
            .ToListAsync();
    }

    private async Task<Dictionary<string, int>> FetchProductQuantities()
    {
        var basketQuantities = await _basketService.GetAllQuantitiesInBasket();
        return ProductList.ToDictionary(p => p.Id!, p => basketQuantities.GetValueOrDefault(p.Id!, 0));
    }
    public async Task<IActionResult> OnPostAddToBasket(string id)
    {
        return await ModifyBasket(id, addItem: true);
    }

    public async Task<IActionResult> OnPostRemoveFromBasket(string id)
    {
        return await ModifyBasket(id, addItem: false);
    }

    private async Task<IActionResult> ModifyBasket(string productId, bool addItem)
    {
        var product = await _context.Products
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(p => p.Id == productId);

        if (product == null)
        {
            ModelState.AddModelError(string.Empty, "Product not found.");
            return Page();
        }

        try
        {
            if (addItem)
            {
                await _basketService.AddToBasket(productId);
                TempData["SuccessMessage"] = "Item successfully added to basket!";
            }
            else
            {
                await _basketService.RemoveFromBasket(productId);
                TempData["SuccessMessage"] = "Item successfully removed from basket!";
            }
        }
        catch (Exception)
        {
            ModelState.AddModelError(string.Empty, "An error occurred while updating the basket.");
        }

        if (ModelState.IsValid)
        {
            return RedirectToPageWithCurrentParameters();
        }

        return Page();
    }

    private RedirectToPageResult RedirectToPageWithCurrentParameters()
    {
        return RedirectToPage("./Index", new
        {
            searchTerm = SearchTerm,
            pageIndex = PageIndex,
            pageSize = PageSize,
            sortBy = SortBy,
            sortOrder = SortOrder
        });
    }
}
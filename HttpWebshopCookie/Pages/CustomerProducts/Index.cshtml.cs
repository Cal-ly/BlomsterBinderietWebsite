namespace HttpWebshopCookie.Pages.CustomerProducts;

public class IndexModel(ApplicationDbContext context, BasketService basketService) : PageModel
{
    [BindProperty]
    public List<ProductViewModel> ProductList { get; set; } = default!;
    public Dictionary<string, int> ProductQuantities { get; set; } = [];
    public string? Occasion { get; set; }
    public string? SearchTerm { get; set; }
    public string? SortBy { get; set; }
    public string? SortOrder { get; set; }
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }

    public async Task OnGetAsync(string? occasion, string searchTerm = "", int pageIndex = 1, int pageSize = 9, string sortBy = "Name", string sortOrder = "Ascending")
    {
        Occasion = occasion;
        SearchTerm = searchTerm ?? string.Empty;
        PageIndex = pageIndex > 0 ? pageIndex : 1;
        PageSize = pageSize > 0 ? pageSize : 9;
        SortBy = !string.IsNullOrEmpty(sortBy) ? sortBy : "Name";
        SortOrder = !string.IsNullOrEmpty(sortOrder) ? sortOrder : "Ascending";

        var query = BuildProductQuery();

        if (!string.IsNullOrEmpty(occasion))
        {
            query = query.Where(p => p.ProductTags.Any(pt => pt.Tag!.Occasion == occasion));
        }

        TotalPages = await CalculateTotalPages(query);
        ProductList = await FetchProducts(query);
        ProductQuantities = await FetchProductQuantities();
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

    private IQueryable<Product> BuildProductQuery()
    {
        var query = context.Products
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
                Price = p.GetPriceString(),
                Description = p.Description,
                ImageUrl = p.ImageUrl,
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
        var basketQuantities = await basketService.GetAllQuantitiesInBasket();
        return ProductList.ToDictionary(p => p.Id!, p => basketQuantities.GetValueOrDefault(p.Id!, 0));
    }

    public async Task<IActionResult> OnPostAddToBasket(string id)
    {
        var productExists = await context.Products.AsNoTracking().AnyAsync(p => p.Id == id);

        if (!productExists)
        {
            TempData["ToastMessage"] = "Error: Product not found.";
            TempData["ToastType"] = "error";
            return RedirectToPageWithCurrentParameters();
        }

        await basketService.AddToBasket(id);
        TempData["ToastMessage"] = "Item successfully added to basket!";
        TempData["ToastType"] = "success";
        return RedirectToPageWithCurrentParameters();
    }

    public async Task<IActionResult> OnPostRemoveFromBasket(string id)
    {
        var productExists = await context.Products.AsNoTracking().AnyAsync(p => p.Id == id);

        if (!productExists)
        {
            TempData["ToastMessage"] = "Error: Product not found.";
            TempData["ToastType"] = "error";
            return RedirectToPageWithCurrentParameters();
        }

        await basketService.RemoveFromBasket(id);
        TempData["ToastMessage"] = "Item successfully removed from basket!";
        TempData["ToastType"] = "warning";
        return RedirectToPageWithCurrentParameters();
    }
}

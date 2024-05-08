namespace HttpWebshopCookie.Pages.Products;

public class IndexModel(ApplicationDbContext context, BasketService basketService) : PageModel
{
    [BindProperty]
    public List<ProductViewModel> ProductList { get; set; } = default!;
    public Dictionary<string, int> ProductQuantities { get; set; } = [];
    public string? SearchTerm { get; set; }
    public string SortBy { get; set; } = "Name";
    public string SortOrder { get; set; } = "Ascending";
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 12;
    public int TotalPages { get; set; }

    public async Task OnGetAsync(string searchTerm, int pageIndex = 1, int pageSize = 12, string sortBy = "Name", string sortOrder = "Ascending")
    {
        SearchTerm = searchTerm;
        PageIndex = pageIndex;
        PageSize = pageSize;
        SortBy = sortBy;
        SortOrder = sortOrder;

        var query = context.Products
            .Include(p => p.ProductTags)
            .ThenInclude(pt => pt.Tag)
            .AsQueryable();

        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(p => p.ProductTags.Any(pt =>
                EF.Functions.Like(pt.Tag!.Occasion, $"%{searchTerm}%") ||
                EF.Functions.Like(pt.Tag!.Category, $"%{searchTerm}%") ||
                EF.Functions.Like(pt.Tag!.SubCategory, $"%{searchTerm}%")));
        }

        query = (sortBy, sortOrder) switch
        {
            ("Price", "Descending") => query.OrderByDescending(p => p.Price),
            ("Price", "Ascending") => query.OrderBy(p => p.Price),
            ("Name", "Descending") => query.OrderByDescending(p => p.Name),
            _ => query.OrderBy(p => p.Name),
        };

        TotalPages = (int)Math.Ceiling(await query.CountAsync() / (double)PageSize);

        ProductList = await query
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
                .Distinct() // Ensures unique tags per product
                .OrderBy(t => t.Category)
                .ThenBy(t => t.SubCategory)
                .ToList()
            })
            .ToListAsync();

        var basketQuantities = await basketService.GetAllQuantitiesInBasket();
        ProductQuantities = ProductList.ToDictionary(p => p.Id!, p => basketQuantities.ContainsKey(p.Id!) ? basketQuantities[p.Id!] : 0);
    }

    public async Task<IActionResult> OnPostAddToBasket(string id)
    {
        if (!await context.Products.AnyAsync(p => p.Id == id))
        {
            return NotFound();
        }

        await basketService.AddToBasket(id);
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostRemoveFromBasket(string id)
    {
        if (!await context.Products.AnyAsync(p => p.Id == id))
        {
            return NotFound();
        }
        await basketService.RemoveFromBasket(id);
        return RedirectToPage();
    }
}

//public async Task OnGetAsync(string searchTerm, int pageIndex = 1, int pageSize = 12, string sortBy = "Name")
//{
//    SearchTerm = searchTerm;
//    PageIndex = pageIndex;
//    PageSize = pageSize;
//    SortBy = sortBy;

//    var products = context.Products
//        .Include(p => p.ProductTags)
//            .ThenInclude(pt => pt.Tag);

//    if (!string.IsNullOrEmpty(searchTerm))
//    {
//        products = (IIncludableQueryable<Product, Tag?>)products.Where(p => p.ProductTags.Any(pt => pt.Tag!.Occasion!.Contains(searchTerm)
//                                                            || pt.Tag!.Category!.Contains(searchTerm)
//                                                            || pt.Tag!.SubCategory!.Contains(searchTerm)));
//    }

//    int totalProducts = await products.CountAsync();
//    TotalPages = (int)Math.Ceiling(totalProducts / (double)PageSize);

//    switch (sortBy)
//    {
//        case "Name":
//            products = (IIncludableQueryable<Product, Tag?>)products.OrderBy(p => p.Name);
//            break;
//        case "Price":
//            products = (IIncludableQueryable<Product, Tag?>)products.OrderBy(p => p.Price);
//            break;
//    }

//    ProductList = await context.Products
//        .Skip((PageIndex - 1) * PageSize)
//        .Take(PageSize)
//        .Include(p => p.ProductTags)
//            .ThenInclude(pt => pt.Tag)
//        .Select(p => new ProductViewModel
//        {
//            Id = p.Id,
//            Name = p.Name,
//            Price = p.Price.ToString("C"),
//            Description = p.Description,
//            Tags = p.ProductTags.Select(pt => new TagViewModel
//            {
//                Occasion = pt.Tag!.Occasion,
//                Category = pt.Tag.Category,
//                SubCategory = pt.Tag.SubCategory
//            })
//            .Distinct() // Ensure uniqueness at this level
//            .OrderBy(t => t.Category) // Optional sorting
//            .ThenBy(t => t.SubCategory) // Optional sorting
//            .ToList()
//        })
//        .OrderBy(p => p.Name)
//        .ToListAsync();

//    var basketQuantities = await basketService.GetAllQuantitiesInBasket();
//    ProductQuantities = ProductList.ToDictionary(p => p.Id!, p => basketQuantities.ContainsKey(p.Id!) ? basketQuantities[p.Id!] : 0);
//}
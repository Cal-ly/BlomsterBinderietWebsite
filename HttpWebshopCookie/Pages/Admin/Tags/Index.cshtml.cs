namespace HttpWebshopCookie.Pages.Admin.Tags
{
    [Authorize(Policy = "managerAccess")]
    public class IndexModel : PageModel
    {
        private readonly TagService _tagService;

        public IndexModel(TagService tagService)
        {
            _tagService = tagService;
        }

        public List<IGrouping<string, Tag>> GroupedTags { get; set; } = new List<IGrouping<string, Tag>>();
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalItems { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);

        public async Task OnGetAsync(int pageNumber = 1, int pageSize = 10)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            var (tags, totalItems) = await _tagService.GetTagsOrderedByOccasionAsync(PageNumber, PageSize);
            GroupedTags = tags.GroupBy(t => t.Occasion).ToList()!;
            TotalItems = totalItems;
        }
    }
}

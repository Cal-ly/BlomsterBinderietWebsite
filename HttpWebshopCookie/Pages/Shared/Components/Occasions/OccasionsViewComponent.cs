namespace HttpWebshopCookie.ViewComponents
{
    public class OccasionsViewComponent : ViewComponent
    {
        private readonly TagService _tagService;

        public OccasionsViewComponent(TagService tagService)
        {
            _tagService = tagService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var occasions = await _tagService.GetOccasionsAsync();
            return View(occasions);
        }
    }
}
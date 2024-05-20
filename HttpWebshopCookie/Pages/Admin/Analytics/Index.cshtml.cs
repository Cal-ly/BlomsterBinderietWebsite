namespace HttpWebshopCookie.Pages.Admin.Analytics;

[Authorize(Policy = "managerAccess")]
public class IndexModel : PageModel
{
    public void OnGet()
    {
    }
}

namespace HttpWebshopCookie.Pages.Admin;

[Authorize(Policy = "staffAccess")]
public class IndexModel : PageModel
{
    public void OnGet()
    {
    }
}

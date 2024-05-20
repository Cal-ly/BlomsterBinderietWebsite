namespace HttpWebshopCookie.Pages.Admin;

[Authorize(Policy = "managerAccess")]
public class IndexModel : PageModel
{
    public void OnGet()
    {
    }
}

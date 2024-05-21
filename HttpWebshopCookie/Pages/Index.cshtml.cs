using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HttpWebshopCookie.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger) //TODO add text and welcome with logo
    {
        _logger = logger;
    }

    public void OnGet()
    {

    }
}

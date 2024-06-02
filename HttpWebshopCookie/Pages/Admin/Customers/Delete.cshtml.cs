namespace HttpWebshopCookie.Pages.Admin.Customers;

[Authorize(Policy = "managerAccess")]
public class DeleteModel(UserManager<Customer> userManager) : PageModel
{
    [BindProperty]
    public Customer CustomerToDelete { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(string id)
    {
        Customer? customer = await userManager.FindByIdAsync(id);
        if (customer == null)
        {
            return NotFound();
        }
        TempData["customerId"] = customer.Id;
        CustomerToDelete = customer;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!TempData.TryGetValue("customerId", out var tempDataValue))
        {
            return NotFound();
        }

        string? customerToDeleteId = tempDataValue!.ToString();
        Customer? customer = await userManager.FindByIdAsync(customerToDeleteId!);
        if (customer == null)
        {
            return NotFound();
        }
        CustomerToDelete = customer;

        var result = await userManager.DeleteAsync(CustomerToDelete);
        if (result.Succeeded)
        {
            return RedirectToPage("Index");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError("Could not delete customer", error.Description);
        }

        return Page();
    }
}

namespace HttpWebshopCookie.Pages.Admin.Employees;

[Authorize(Policy = "managerAccess")]
public class DeleteModel(UserManager<Employee> userManager, ApplicationDbContext context) : PageModel
{
    [BindProperty]
    public Employee EmployeeToDelete { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(string id)
    {
        Employee? userToDelete = await userManager.FindByIdAsync(id);
        if (userToDelete == null)
        {
            return NotFound();
        }
        TempData["employeeId"] = userToDelete.Id;
        EmployeeToDelete = userToDelete;

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!TempData.TryGetValue("employeeId", out var tempDataValue))
        {
            return NotFound();
        }
        string? employeeToDeleteId = tempDataValue!.ToString();
        Employee? employeeToDelete = await userManager.FindByIdAsync(employeeToDeleteId!);
        if (employeeToDelete == null)
        {
            return NotFound();
        }
        EmployeeToDelete = employeeToDelete;
        
        var orders = context.Orders.Where(o => o.EmployeeId == EmployeeToDelete.Id);
        foreach (var order in orders)
        {
            order.CustomerId = null;
            order.Customer = null;
        }
        await context.SaveChangesAsync();

        var result = await userManager.DeleteAsync(EmployeeToDelete);
        if (result.Succeeded)
        {
            return RedirectToPage("Index");
        }
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError("Could not delete employee", error.Description);
        }
        return Page();
    }
}
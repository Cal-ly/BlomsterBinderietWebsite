namespace HttpWebshopCookie.Pages.Admin.Employees;

[Authorize(Policy = "managerAccess")]
public class DeleteModel(UserManager<Employee> userManager) : PageModel
{
    [BindProperty]
    public Employee UserToDelete { get; set; } = null!;

    public async Task<IActionResult> OnGetAsync(string id)
    {
        Employee? userToDelete = await userManager.FindByIdAsync(id);
        if (userToDelete == null)
        {
            return NotFound();
        }
        TempData["employeeId"] = userToDelete.Id;
        UserToDelete = userToDelete;

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
        UserToDelete = employeeToDelete;

        var result = await userManager.DeleteAsync(UserToDelete);
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
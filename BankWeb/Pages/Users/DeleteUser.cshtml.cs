using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

[Authorize(Roles = "Admin")]
public class DeleteUserModel(UserManager<IdentityUser> userManager) : PageModel
{
    [BindProperty]
    public string Id { get; set; }

    public string Email { get; set; }

    public async Task<IActionResult> OnGetAsync(string id)
    {
        var user = await userManager.FindByIdAsync(id);
        Id = user.Id;
        Email = user.Email;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var user = await userManager.FindByIdAsync(Id);
        var result = await userManager.DeleteAsync(user);
        if (result.Succeeded)
        {
            TempData["UserDeletedMessage"] = $"User {Email} was successfully deleted.";
            return RedirectToPage("ManageUsers");
        }

        return Page();
    }

    public async Task<List<string>> GetRoles()
    {
        var user = await userManager.FindByEmailAsync(Email);
        var roles = await userManager.GetRolesAsync(user);
        return roles.ToList();
    }

}

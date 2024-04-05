using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

public class DeleteUserModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;

    [BindProperty]
    public string Id { get; set; }

    public string Email { get; set; }

    public DeleteUserModel(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IActionResult> OnGetAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        Id = user.Id;
        Email = user.Email;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var user = await _userManager.FindByIdAsync(Id);
        var result = await _userManager.DeleteAsync(user);
        if (result.Succeeded)
        {
            TempData["UserDeletedMessage"] = $"User {Email} was successfully deleted.";
            return RedirectToPage("ManageUsers");
        }

        return Page();
    }

    public async Task<List<string>> GetRoles()
    {
        var user = await _userManager.FindByEmailAsync(Email);
        var roles = await _userManager.GetRolesAsync(user);
        return roles.ToList();
    }

}

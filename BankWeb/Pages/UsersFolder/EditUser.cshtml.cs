using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

public class EditUserModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;

    [BindProperty]
    public string Id { get; set; }

    [BindProperty, Required, EmailAddress]
    public string Email { get; set; }

    public EditUserModel(UserManager<IdentityUser> userManager)
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
        user.Email = Email;
        user.UserName = Email;
        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded)
        {
            return RedirectToPage("ManageUsers");
        }
        return Page();
    }
}

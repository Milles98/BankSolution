using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

public class CreateUserModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;

    [BindProperty, Required, EmailAddress]
    public string Email { get; set; }

    [BindProperty, Required, MinLength(6)]
    public string Password { get; set; }

    public CreateUserModel(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var user = new IdentityUser { UserName = Email, Email = Email };
        var result = await _userManager.CreateAsync(user, Password);
        if (result.Succeeded)
        {
            return RedirectToPage("ManageUsers");
        }
        return Page();
    }
}

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;

[Authorize(Roles = "Admin")]
public class EditUserModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    [BindProperty]
    public string Id { get; set; }

    [BindProperty, Required, EmailAddress]
    public string Email { get; set; }

    [BindProperty]
    public string Password { get; set; }

    [BindProperty, Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; }

    [BindProperty]
    public string Role { get; set; }

    public EditUserModel(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
    }

    public async Task<IActionResult> OnGetAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        Id = user.Id;
        Email = user.Email;
        var userRole = await _userManager.GetRolesAsync(user);
        Role = userRole.FirstOrDefault();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var user = await _userManager.FindByIdAsync(Id);
        user.Email = Email;
        user.UserName = Email;
        if (!string.IsNullOrEmpty(Password))
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetResult = await _userManager.ResetPasswordAsync(user, token, Password);
            if (!resetResult.Succeeded)
            {
                foreach (var error in resetResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }
        }
        var updateResult = await _userManager.UpdateAsync(user);
        if (updateResult.Succeeded)
        {
            var userRole = await _userManager.GetRolesAsync(user);
            if (userRole.FirstOrDefault() != Role)
            {
                await _userManager.RemoveFromRoleAsync(user, userRole.FirstOrDefault());
                await _userManager.AddToRoleAsync(user, Role);
            }
            await _signInManager.RefreshSignInAsync(user);
            return RedirectToPage("ManageUsers");
        }
        foreach (var error in updateResult.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
        return Page();
    }
}

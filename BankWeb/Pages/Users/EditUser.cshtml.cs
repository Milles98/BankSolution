using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;

[Authorize(Roles = "Admin")]
public class EditUserModel(
    UserManager<IdentityUser> userManager,
    SignInManager<IdentityUser> signInManager,
    RoleManager<IdentityRole> roleManager)
    : PageModel
{
    private readonly RoleManager<IdentityRole> _roleManager = roleManager;

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

    public async Task<IActionResult> OnGetAsync(string id)
    {
        var user = await userManager.FindByIdAsync(id);
        Id = user.Id;
        Email = user.Email;
        var userRole = await userManager.GetRolesAsync(user);
        Role = userRole.FirstOrDefault();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var user = await userManager.FindByIdAsync(Id);
        user.Email = Email;
        user.UserName = Email;
        if (!string.IsNullOrEmpty(Password))
        {
            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var resetResult = await userManager.ResetPasswordAsync(user, token, Password);
            if (!resetResult.Succeeded)
            {
                foreach (var error in resetResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }
        }
        var updateResult = await userManager.UpdateAsync(user);
        if (updateResult.Succeeded)
        {
            var userRole = await userManager.GetRolesAsync(user);
            if (userRole.FirstOrDefault() != Role)
            {
                await userManager.RemoveFromRoleAsync(user, userRole.FirstOrDefault());
                await userManager.AddToRoleAsync(user, Role);
            }
            await signInManager.RefreshSignInAsync(user);
            return RedirectToPage("ManageUsers");
        }
        foreach (var error in updateResult.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
        return Page();
    }
}

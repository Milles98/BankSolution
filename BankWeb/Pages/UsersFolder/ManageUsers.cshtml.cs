using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BankWeb.Pages.UsersFolder
{
    [Authorize(Roles = "Admin")]
    public class ManageUsersModel(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        : PageModel
    {
        public IList<IdentityUser> Users { get; set; }
        public string? LoggedInUserId { get; set; }

        public async Task OnGetAsync()
        {
            Users = await userManager.Users.ToListAsync();
            var loggedInUser = await signInManager.UserManager.GetUserAsync(User);
            LoggedInUserId = loggedInUser?.Id;
        }

        public async Task<IList<string>> GetRoles(IdentityUser user)
        {
            return await userManager.GetRolesAsync(user);
        }
    }

}

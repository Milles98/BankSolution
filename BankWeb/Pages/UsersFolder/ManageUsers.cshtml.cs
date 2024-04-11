using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BankWeb.Pages.UsersFolder
{
    [Authorize(Roles = "Admin")]
    public class ManageUsersModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public ManageUsersModel(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IList<IdentityUser> Users { get; set; }
        public string? LoggedInUserId { get; set; }

        public async Task OnGetAsync()
        {
            Users = await _userManager.Users.ToListAsync();
            var loggedInUser = await _signInManager.UserManager.GetUserAsync(User);
            LoggedInUserId = loggedInUser?.Id;
        }

        public async Task<IList<string>> GetRoles(IdentityUser user)
        {
            return await _userManager.GetRolesAsync(user);
        }
    }

}

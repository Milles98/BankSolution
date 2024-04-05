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

        public ManageUsersModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public IList<IdentityUser> Users { get; set; }

        public async Task OnGetAsync()
        {
            Users = await _userManager.Users.ToListAsync();
        }

        public async Task<IList<string>> GetRoles(IdentityUser user)
        {
            return await _userManager.GetRolesAsync(user);
        }
    }
}

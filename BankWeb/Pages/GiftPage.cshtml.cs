using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BankWeb.Pages
{
    [Authorize(Roles = "Admin")]
    public class GiftPageModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}

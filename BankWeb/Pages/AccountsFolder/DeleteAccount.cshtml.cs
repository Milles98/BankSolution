using DataLibrary.Services.Interfaces;
using DataLibrary.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BankWeb.Pages.AccountsFolder
{
    public class DeleteAccountModel(IAccountService accountService) : PageModel
    {
        [BindProperty]
        public AccountViewModel Account { get; set; }

        public IActionResult OnGet(int id)
        {
            var accountIds = new List<int> { id };
            var accounts = accountService.GetAccountDetails(accountIds);

            Account = accounts.First();

            if (Account == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            try
            {
                await accountService.DeleteAccountAndRelatedData(id);
                TempData["Message"] = "Account and related data successfully deleted.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Failed to delete account: " + ex.Message;
            }

            return RedirectToPage("/AccountsFolder/AccountsAdminInfo");
        }


    }
}
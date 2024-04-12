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

            TempData.Keep("Message");
            TempData.Keep("MessageClass");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var accountIds = new List<int> { id };
            var accounts = accountService.GetAccountDetails(accountIds);

            Account = accounts.First();

            if (Account == null)
            {
                return NotFound();
            }

            if (Account.Balance > 0)
            {
                TempData["Message"] = "Account has a balance. Please withdraw or transfer the funds before deleting the account.";
                TempData["MessageClass"] = "alert-danger";
                TempData["ShowWithdrawButton"] = true;
                TempData["ShowTransferButton"] = true;
                return Page();
            }


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
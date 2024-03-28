using BankWeb.ViewModels;
using DataLibrary.Data;
using DataLibrary.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Identity.Client;

namespace BankWeb.Pages
{
    public class WithdrawModel : PageModel
    {
        private readonly IBankService _bankService;
        private readonly BankAppData2Context _context;

        [BindProperty]
        public int AccountId { get; set; }

        [BindProperty]
        public decimal Amount { get; set; }
        public AccountViewModel Account { get; set; }

        public WithdrawModel(IBankService bankService, BankAppData2Context context)
        {
            _bankService = bankService;
            _context = context;

        }

        public void OnGet(int accountId = 0)
        {
            AccountId = accountId;
            if (accountId > 0)
            {
                var account = _context.Accounts.FirstOrDefault(a => a.AccountId == accountId);
                if (account != null)
                {
                    Account = new AccountViewModel
                    {
                        AccountId = account.AccountId.ToString(),
                        Frequency = account.Frequency,
                        Created = account.Created.ToString(),
                        Balance = account.Balance,
                        Type = account.GetType().Name
                    };
                }
            }
        }

        public IActionResult OnPost()
        {
            try
            {
                if (Amount <= 0)
                {
                    TempData["Message"] = "Withdraw amount must be greater than 0!";
                    TempData["MessageClass"] = "alert-danger";
                    return Page();
                }
                else if (Amount >= 50000)
                {
                    TempData["Message"] = "Withdraw amount must be less than 50,000 SEK!";
                    TempData["MessageClass"] = "alert-danger";
                    return Page();
                }

                var transactionId = _bankService.Withdraw(AccountId, Amount);
                if (transactionId > 0)
                {
                    TempData["Message"] = $"Withdraw successful for Account ID {AccountId}, Amount: -{Amount} SEK, Date: {DateTime.Now:dd-MM-yyyy}, " +
                      $"Transaction ID: <a href=\"/TransactionDetails?transactionId={transactionId}\">{transactionId}</a>";
                    TempData["MessageClass"] = "alert-success";
                }
                else
                {
                    TempData["Message"] = "Withdraw failed!";
                    TempData["MessageClass"] = "alert-danger";
                }

                var account = _context.Accounts.FirstOrDefault(a => a.AccountId == AccountId);
                if (account != null)
                {
                    Account = new AccountViewModel
                    {
                        AccountId = account.AccountId.ToString(),
                        Frequency = account.Frequency,
                        Created = account.Created.ToString(),
                        Balance = account.Balance,
                        Type = account.GetType().Name
                    };
                }
            }
            catch (Exception ex)
            {
                TempData["Message"] = $"An error occurred: {ex.Message}";
                TempData["MessageClass"] = "alert-danger";
            }

            return Page();
        }

    }
}

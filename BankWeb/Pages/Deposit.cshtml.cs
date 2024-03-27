using DataLibrary.Data;
using DataLibrary.Services.Interfaces;
using DataLibrary.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BankWeb.Pages
{
    public class DepositModel : PageModel
    {
        private readonly IBankService _bankService;
        private readonly BankAppData2Context _context;

        [BindProperty]
        public int AccountId { get; set; }

        [BindProperty]
        public decimal Amount { get; set; }
        public AccountViewModel Account { get; set; }

        public DepositModel(IBankService bankService, BankAppData2Context context)
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
            var transactionId = _bankService.Deposit(AccountId, Amount);
            if (transactionId > 0)
            {
                TempData["Message"] = "Deposit successful!";
                TempData["MessageClass"] = "alert-success";
            }
            else
            {
                TempData["Message"] = "Deposit failed!";
                TempData["MessageClass"] = "alert-danger";
            }

            // Set the Account property
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

            return Page();
        }

    }

}

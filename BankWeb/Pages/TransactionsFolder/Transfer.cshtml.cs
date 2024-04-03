using DataLibrary.Data;
using DataLibrary.Services.Interfaces;
using DataLibrary.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BankWeb.Pages.TransactionsFolder
{
    [Authorize(Roles = "Admin")]
    public class TransferModel : PageModel
    {
        private readonly IBankService _bankService;
        private readonly BankAppDataContext _context;

        [BindProperty]
        public int AccountId { get; set; }


        [BindProperty]
        public decimal Amount { get; set; }
        public AccountViewModel Account { get; set; }

        [BindProperty]
        public int ToAccountId { get; set; }


        public TransferModel(IBankService bankService, BankAppDataContext context)
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
                var fromAccount = _context.Accounts.FirstOrDefault(a => a.AccountId == AccountId);
                if (fromAccount == null)
                {
                    TempData["Message"] = "From Account ID does not exist!";
                    TempData["MessageClass"] = "alert-danger";
                    return Page();
                }

                // Check if ToAccountId exists in the database
                var toAccount = _context.Accounts.FirstOrDefault(a => a.AccountId == ToAccountId);
                if (toAccount == null)
                {
                    TempData["Message"] = "To Account ID does not exist!";
                    TempData["MessageClass"] = "alert-danger";
                    return Page();
                }

                if (AccountId == ToAccountId)
                {
                    TempData["Message"] = "Cannot transfer to the same account!";
                    TempData["MessageClass"] = "alert-danger";
                    return Page();
                }
                else if (Amount <= 0)
                {
                    TempData["Message"] = "Amount must be greater than 0!";
                    TempData["MessageClass"] = "alert-danger";
                    return Page();
                }
                else if (Amount > fromAccount.Balance)
                {
                    TempData["Message"] = "Amount exceeds account balance!";
                    TempData["MessageClass"] = "alert-danger";
                    return Page();
                }


                // Call Transfer method
                var transactionId = _bankService.Transfer(AccountId, ToAccountId, Amount);

                // Update the Account view model
                fromAccount = _context.Accounts.First(a => a.AccountId == AccountId);
                if (fromAccount != null)
                {
                    Account = new AccountViewModel
                    {
                        AccountId = fromAccount.AccountId.ToString(),
                        Frequency = fromAccount.Frequency,
                        Created = fromAccount.Created.ToString(),
                        Balance = fromAccount.Balance,
                        Type = fromAccount.GetType().Name
                    };
                }

                TempData["Message"] = $"Transfer successful for Account ID {AccountId} to Account ID {ToAccountId}, Amount: {Amount} SEK, Date: {DateTime.Now:dd-MM-yyyy}, " +
                    $"Transaction ID: <a href=\"/TransactionsFolder/TransactionDetails?transactionId={transactionId}\">{transactionId}</a>";
                TempData["MessageClass"] = "alert-success";
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

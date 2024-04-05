using DataLibrary.Data;
using DataLibrary.Services.Interfaces;
using DataLibrary.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BankWeb.Pages.TransactionsFolder
{
    [Authorize(Roles = "Cashier")]
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
                Account = _bankService.GetAccountDetailsForDisplay(accountId);
            }
            TempData["Account"] = System.Text.Json.JsonSerializer.Serialize(Account);
        }

        public IActionResult OnPost()
        {
            try
            {
                var transactionId = _bankService.TransferFunds(AccountId, ToAccountId, Amount);
                TempData["Message"] = $"Transfer successful from Account ID {AccountId} to Account ID {ToAccountId}, Amount: {Amount} SEK, Date: {DateTime.Now:dd-MM-yyyy}, " +
                    $"Transaction ID: <a href=\"/TransactionsFolder/TransactionDetails?transactionId={transactionId}\">{transactionId}</a>";
                TempData["MessageClass"] = "alert-success";

                Account = _bankService.GetAccountDetailsForDisplay(AccountId);
            }
            catch (Exception ex)
            {
                TempData["Message"] = $"An error occurred: {ex.Message}";
                TempData["MessageClass"] = "alert-danger";
                Account = System.Text.Json.JsonSerializer.Deserialize<AccountViewModel>((string)TempData.Peek("Account"));
            }

            return Page();
        }



    }
}

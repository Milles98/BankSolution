using DataLibrary.Data;
using DataLibrary.Services.Interfaces;
using DataLibrary.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Identity.Client;

namespace BankWeb.Pages.TransactionsFolder
{
    public class WithdrawModel : PageModel
    {
        private readonly IBankService _bankService;

        [BindProperty]
        public int AccountId { get; set; }

        [BindProperty]
        public decimal Amount { get; set; }
        public AccountViewModel Account { get; set; }

        public WithdrawModel(IBankService bankService)
        {
            _bankService = bankService;

        }

        public void OnGet(int accountId = 0)
        {
            AccountId = accountId;
            Account = _bankService.GetAccountDetailsForDisplay(accountId);
        }


        public IActionResult OnPost()
        {
            try
            {
                var transactionId = _bankService.Withdraw(AccountId, Amount);
                TempData["Message"] = $"Withdraw successful for Account ID {AccountId}, Amount: -{Amount} SEK, Date: {DateTime.Now:dd-MM-yyyy}, " +
                  $"Transaction ID: <a href=\"/TransactionDetails?transactionId={transactionId}\">{transactionId}</a>";
                TempData["MessageClass"] = "alert-success";

                Account = _bankService.GetAccountDetails(AccountId);
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
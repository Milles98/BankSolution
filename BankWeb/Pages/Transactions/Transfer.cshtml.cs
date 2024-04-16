using DataLibrary.Data;
using DataLibrary.Services.Interfaces;
using DataLibrary.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace BankWeb.Pages.Transactions
{
    [Authorize(Roles = "Cashier")]
    public class TransferModel(IBankService bankService) : PageModel
    {
        [BindProperty]
        [Required]
        public int AccountId { get; set; }


        [BindProperty]
        [Range(50, 50000)]
        [Required]
        public decimal Amount { get; set; }
        public AccountViewModel Account { get; set; }

        [BindProperty]
        public int ToAccountId { get; set; }


        public void OnGet(int accountId = 0)
        {
            AccountId = accountId;
            if (accountId > 0)
            {
                Account = bankService.GetAccountDetailsForDisplay(accountId);
            }
            TempData["Account"] = System.Text.Json.JsonSerializer.Serialize(Account);
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            
            try
            {
                var transactionId = bankService.TransferFunds(AccountId, ToAccountId, Amount);
                TempData["BankMessage"] = $"Transfer successful from Account ID {AccountId} to Account ID {ToAccountId}, Amount: {Amount} SEK, Date: {DateTime.Now:dd-MM-yyyy}, " +
                    $"Transaction ID: <a href=\"/Transactions/TransactionDetails?transactionId={transactionId}\">{transactionId}</a>";
                TempData["MessageClass"] = "alert-success";

                Account = bankService.GetAccountDetailsForDisplay(AccountId);
                
                return RedirectToPage("/Customers/CustomerDetails", new { id = Account.CustomerId });
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

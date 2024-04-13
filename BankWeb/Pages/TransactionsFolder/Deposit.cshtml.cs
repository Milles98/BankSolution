using DataLibrary.Data;
using DataLibrary.Services.Interfaces;
using DataLibrary.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace BankWeb.Pages.TransactionsFolder
{
    [Authorize(Roles = "Cashier")]
    public class DepositModel(IBankService bankService) : PageModel
    {
        [BindProperty]
        [Required]
        public int AccountId { get; set; }

        [BindProperty]
        [Range(50, 50000)]
        [Required]
        public decimal Amount { get; set; }
        public AccountViewModel Account { get; set; }

        public void OnGet(int accountId)
        {
            AccountId = accountId;
            Account = bankService.GetAccountDetailsForDisplay(accountId);
            TempData["Account"] = System.Text.Json.JsonSerializer.Serialize(Account);
        }

        public IActionResult OnPost()
        {
            try
            {
                var transactionId = bankService.DepositFunds(AccountId, Amount);
                TempData["Message"] = $"Deposit successful for Account ID {AccountId}, Amount: {Amount} SEK, Date: {DateTime.Now:dd-MM-yyyy}, " +
                    $"Transaction ID: <a href=\"/TransactionsFolder/TransactionDetails?transactionId={transactionId}\">{transactionId}</a>";
                TempData["MessageClass"] = "alert-success";

                Account = bankService.GetAccountDetails(AccountId);
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

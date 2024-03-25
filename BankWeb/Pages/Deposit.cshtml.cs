using BankWeb.Services;
using BankWeb.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BankWeb.Pages
{
    public class DepositModel : PageModel
    {
        private readonly IBankService _bankService;

        [BindProperty]
        public int AccountId { get; set; }

        [BindProperty]
        public decimal Amount { get; set; }

        public DepositModel(IBankService bankService)
        {
            _bankService = bankService;
        }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            var transactionId = _bankService.Deposit(AccountId, Amount);
            return RedirectToPage("DepositReceipt", new { transactionId = transactionId, accountId = AccountId, amount = Amount, time = DateTime.Now });
        }
    }

}

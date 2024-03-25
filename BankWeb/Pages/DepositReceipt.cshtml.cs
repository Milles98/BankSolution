using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BankWeb.Pages
{
    public class DepositReceiptModel : PageModel
    {
        public int TransactionId { get; set; }
        public int AccountId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Time { get; set; }

        public void OnGet(int transactionId, int accountId, decimal amount, DateTime time)
        {
            TransactionId = transactionId;
            AccountId = accountId;
            Amount = amount;
            Time = time;
        }
    }
}

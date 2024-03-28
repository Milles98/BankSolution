using BankWeb.ViewModels;
using DataLibrary.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BankWeb.Pages
{
    public class TransactionDetailsModel : PageModel
    {
        private readonly BankAppData2Context _context;

        public TransactionViewModel Transaction { get; set; }

        public TransactionDetailsModel(BankAppData2Context context)
        {
            _context = context;
        }

        public void OnGet(int transactionId)
        {
            var transaction = _context.Transactions.FirstOrDefault(t => t.TransactionId == transactionId);
            if (transaction != null)
            {
                Transaction = new TransactionViewModel
                {
                    TransactionId = transaction.TransactionId,
                    AccountId = transaction.AccountId,
                    Amount = transaction.Amount,
                    Balance = transaction.Balance,
                    DateOfTransaction = transaction.Date,
                    Operation = transaction.Operation
                };
            }
        }
    }

}

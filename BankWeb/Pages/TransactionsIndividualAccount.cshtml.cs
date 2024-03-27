using DataLibrary.Data;
using DataLibrary.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BankWeb.Pages
{
    public class TransactionsIndividualAccountModel : PageModel
    {
        private readonly BankAppData2Context _context;
        private readonly ILogger<TransactionsIndividualAccountModel> _logger;
        public TransactionsIndividualAccountModel(BankAppData2Context context, ILogger<TransactionsIndividualAccountModel> logger)
        {
            _context = context;
            _logger = logger;
        }
        public int AccountId { get; set; }
        public decimal Balance { get; set; }
        public List<TransactionViewModel> Transactions { get; set; }
        public void OnGet(int accountId)
        {
            AccountId = accountId;
            var account = _context.Accounts.Include(a => a.Transactions).FirstOrDefault(a => a.AccountId == accountId);
            Balance = account.Balance;
            Transactions = account.Transactions
                .OrderByDescending(t => t.Date)
                .Take(20)
                .Select(t => new TransactionViewModel
                {
                    TransactionId = t.TransactionId,
                    DateOfTransaction = t.Date,
                    Type = t.Type,
                    Operation = t.Operation,
                    Amount = t.Amount,
                    Balance = t.Balance
                }).ToList();
        }

        public JsonResult OnGetLoadMoreTransactions(int accountId, int page)
        {
            if (page < 1)
            {
                // Return an error response or set page to 1
                page = 1;
            }

            try
            {
                var transactions = _context.Transactions
                    .Where(t => t.AccountId == accountId)
                    .OrderByDescending(t => t.Date)
                    .Skip((page - 1) * 20)
                    .Take(20)
                    .Select(t => new TransactionViewModel
                    {
                        TransactionId = t.TransactionId,
                        DateOfTransaction = t.Date,
                        Type = t.Type,
                        Operation = t.Operation,
                        Amount = t.Amount,
                        Balance = t.Balance
                    }).ToList();

                return new JsonResult(transactions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load more transactions.");
                // Log the exception details to your logging framework or storage
                // For example: _logger.LogError(ex, "Failed to load more transactions.");

                // Return a JsonResult indicating an error occurred
                // You might want to customize the status code and message based on the exception or use a generic error message
                return new JsonResult(new { error = "An error occurred while processing your request. " + ex.Message })
                {
                    StatusCode = 500 // Internal Server Error
                };
            }
        }
    }
}

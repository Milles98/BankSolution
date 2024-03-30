using DataLibrary.Data;
using DataLibrary.Services.Interfaces;
using DataLibrary.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BankWeb.Pages.TransactionsFolder
{
    public class TransactionsIndividualAccountModel : PageModel
    {
        private readonly ITransactionService _transactionService;
        private readonly BankAppData2Context _context;

        public TransactionsIndividualAccountModel(ITransactionService transactionService, BankAppData2Context context)
        {
            _transactionService = transactionService;
            _context = context;
        }

        public int AccountId { get; set; }
        public decimal Balance { get; set; }
        public List<TransactionViewModel> Transactions { get; set; }
        public async Task OnGet(int accountId)
        {
            AccountId = accountId;
            Balance = await _transactionService.GetAccountBalance(accountId);
            Transactions = await _transactionService.GetTransactionsForAccount(accountId, 1);
        }

        public JsonResult OnGetLoadMoreTransactions(int accountId, int page)
        {
            if (page < 1)
            {
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
                return new JsonResult(new { error = "An error occurred while processing your request. " + ex.Message })
                {
                    StatusCode = 500
                };
            }
        }
    }
}

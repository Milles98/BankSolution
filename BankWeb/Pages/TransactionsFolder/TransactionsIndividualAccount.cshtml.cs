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
        private readonly BankAppDataContext _context;

        public TransactionsIndividualAccountModel(ITransactionService transactionService, BankAppDataContext context)
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
            Transactions = await _transactionService.GetTransactionsForAccount(accountId, null);
        }

        public JsonResult OnGetLoadMoreTransactions(int accountId, DateTime? lastFetchedTransactionTimestamp, string loadedTransactionIds)
        {
            var loadedIds = new List<int>();
            if (!string.IsNullOrEmpty(loadedTransactionIds))
            {
                loadedIds = loadedTransactionIds.Split(',').Select(int.Parse).ToList();
            }

            try
            {
                var transactionsQuery = _context.Transactions
                    .Where(t => t.AccountId == accountId && !loadedIds.Contains(t.TransactionId));

                if (lastFetchedTransactionTimestamp.HasValue)
                {
                    var lastFetchedDate = DateOnly.FromDateTime(lastFetchedTransactionTimestamp.Value);
                    transactionsQuery = transactionsQuery
                        .Where(t => t.Date < lastFetchedDate);
                }


                var transactions = transactionsQuery
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
                    })
                    .ToList();

                var hasMore = transactionsQuery
                    .OrderByDescending(t => t.Date)
                    .Skip(20)
                    .Any();

                return new JsonResult(new { transactions, hasMore });
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

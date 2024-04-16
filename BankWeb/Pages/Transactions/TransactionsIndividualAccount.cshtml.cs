using DataLibrary.Data;
using DataLibrary.Services.Interfaces;
using DataLibrary.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BankWeb.Pages.Transactions
{
    [Authorize(Roles = "Cashier")]
    public class TransactionsIndividualAccountModel(ITransactionService transactionService) : PageModel
    {
        public int AccountId { get; set; }
        public decimal? Balance { get; set; }
        public string SearchTerm { get; set; }
        public List<TransactionViewModel> Transactions { get; set; }
        public async Task<IActionResult> OnGet(int accountId, string search = null)
        {
            AccountId = accountId;
            SearchTerm = search;
            Balance = await transactionService.GetAccountBalance(accountId);
            Transactions = await transactionService.GetTransactionsForAccount(accountId, null);
            if (!string.IsNullOrEmpty(SearchTerm))
            {
                var exactMatch = Transactions.SingleOrDefault(t => t.TransactionId.ToString() == SearchTerm);
                if (exactMatch == null)
                {
                    ModelState.AddModelError("", "Could not find exact match. Try again.");
                }
                Transactions = Transactions.Where(t => t.TransactionId.ToString().Contains(SearchTerm)).ToList();
                if (Transactions.Count == 0)
                {
                    ModelState.AddModelError("", "Transaction not found.");
                }
            }
            return Page();
        }


        public async Task<JsonResult> OnGetLoadMoreTransactions(int accountId, DateTime? lastFetchedTransactionTimestamp, string loadedTransactionIds)
        {
            try
            {
                var (transactions, hasMore) = await transactionService.LoadMoreTransactions(accountId, lastFetchedTransactionTimestamp, loadedTransactionIds);
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

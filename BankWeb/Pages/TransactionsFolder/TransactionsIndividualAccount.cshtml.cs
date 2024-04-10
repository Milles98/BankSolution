using DataLibrary.Data;
using DataLibrary.Services.Interfaces;
using DataLibrary.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BankWeb.Pages.TransactionsFolder
{
    [Authorize(Roles = "Cashier")]
    public class TransactionsIndividualAccountModel : PageModel
    {
        private readonly ITransactionService _transactionService;

        public TransactionsIndividualAccountModel(ITransactionService transactionService)
        {
            _transactionService = transactionService;
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

        public async Task<JsonResult> OnGetLoadMoreTransactions(int accountId, DateTime? lastFetchedTransactionTimestamp, string loadedTransactionIds)
        {
            try
            {
                var (transactions, hasMore) = await _transactionService.LoadMoreTransactions(accountId, lastFetchedTransactionTimestamp, loadedTransactionIds);
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

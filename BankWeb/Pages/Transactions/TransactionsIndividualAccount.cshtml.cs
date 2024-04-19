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
        public string Search { get; set; }
        public List<TransactionViewModel> Transactions { get; set; }
        public async Task<IActionResult> OnGet(string sortColumn, string sortOrder, int accountId, string query)
        {
            Search = query;
            AccountId = accountId;
            Balance = await transactionService.GetAccountBalance(accountId);
            Transactions = await transactionService.GetTransactionsForIndividualAccount(sortColumn, sortOrder, accountId, query);

            return Page();
        }


        public async Task<JsonResult> OnGetLoadMoreTransactions(int accountId, string loadedTransactionIds, int pageNo)
        {
            try
            {
                var (transactions, hasMore) = await transactionService.LoadMoreTransactions(accountId, loadedTransactionIds, pageNo);
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

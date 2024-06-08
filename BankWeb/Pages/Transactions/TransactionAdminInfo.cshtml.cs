using DataLibrary.Data;
using DataLibrary.Infrastructure.Paging;
using DataLibrary.Services;
using DataLibrary.Services.Interfaces;
using DataLibrary.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BankWeb.Pages.Transactions
{
    // [Authorize(Roles = "Cashier")]
    public class TransactionAdminInfoModel(ITransactionService transactionService) : PageModel
    {
        public PagedResult<TransactionViewModel> Transactions { get; set; }
        public int CurrentPage { get; set; }
        public int TransactionsPerPage { get; set; }
        public int TotalTransactions { get; set; }
        public string Search { get; set; }

        public async Task OnGet(string sortColumn, string sortOrder, int pageNo, string query)
        {
            Search = query;

            if (pageNo == 0)
                pageNo = 1;
            CurrentPage = pageNo;

            if (Request.Query.ContainsKey("page"))
            {
                CurrentPage = int.Parse(Request.Query["page"]);
            }

            TotalTransactions = transactionService.GetTotalTransactions();

            Transactions = await transactionService
                .GetTransactions(CurrentPage, TransactionsPerPage, sortColumn, sortOrder, query);
        }
    }
}

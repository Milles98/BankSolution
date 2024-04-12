using DataLibrary.Data;
using DataLibrary.Services;
using DataLibrary.Services.Interfaces;
using DataLibrary.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BankWeb.Pages.TransactionsFolder
{
    [Authorize(Roles = "Cashier")]
    public class TransactionAdminInfoModel(ITransactionService transactionService) : PageModel
    {
        public List<TransactionViewModel> Transactions { get; set; }

        public int CurrentPage { get; set; } = 1;
        public int TransactionsPerPage { get; set; } = 50;
        public int TotalPages => transactionService.GetTotalPages(TransactionsPerPage);
        public async Task OnGet(string sortColumn, string sortOrder, string search)
        {
            if (Request.Query.ContainsKey("page"))
            {
                CurrentPage = int.Parse(Request.Query["page"]);
            }

            Transactions = await transactionService
                .GetTransactions(CurrentPage, TransactionsPerPage, sortColumn, sortOrder, search);
        }
    }
}

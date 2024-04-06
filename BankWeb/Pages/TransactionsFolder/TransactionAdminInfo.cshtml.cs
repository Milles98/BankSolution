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
    public class TransactionAdminInfoModel : PageModel
    {
        private readonly ITransactionService _transactionService;

        public TransactionAdminInfoModel(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }
        public List<TransactionViewModel> Transactions { get; set; }

        public int CurrentPage { get; set; } = 1;
        public int TransactionsPerPage { get; set; } = 50;
        public int TotalPages => _transactionService.GetTotalPages(TransactionsPerPage);
        public async Task OnGet(string sortColumn, string sortOrder, string search)
        {
            if (Request.Query.ContainsKey("page"))
            {
                CurrentPage = int.Parse(Request.Query["page"]);
            }

            Transactions = await _transactionService
                .GetTransactions(CurrentPage, TransactionsPerPage, sortColumn, sortOrder, search);
        }
    }
}

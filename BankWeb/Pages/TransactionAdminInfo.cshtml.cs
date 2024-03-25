using BankWeb.Data;
using BankWeb.Services;
using BankWeb.Services.Interfaces;
using BankWeb.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq.Expressions;

namespace BankWeb.Pages
{
    [Authorize(Roles = "Admin")]
    public class TransactionAdminInfoModel : PageModel
    {
        private readonly IPaginationService<Transaction> _paginationService;
        private readonly BankAppData2Context _context;
        private readonly ISortingService<Transaction> _sortingService;

        public TransactionAdminInfoModel(IPaginationService<Transaction> paginationService,
            ISortingService<Transaction> sortingService, BankAppData2Context context)
        {
            _paginationService = paginationService;
            _sortingService = sortingService;
            _context = context;
        }
        public List<TransactionViewModel> Transactions { get; set; } = new();

        public int CurrentPage { get; set; } = 1;
        public int TransactionsPerPage { get; set; } = 7;
        public int TotalPages => (int)Math.Ceiling(_context.Transactions.Count() / (double)TransactionsPerPage);
        public void OnGet(string sortColumn, string sortOrder, string search)
        {
            if (Request.Query.ContainsKey("page"))
            {
                CurrentPage = int.Parse(Request.Query["page"]);
            }

            var query = _context.Transactions.AsQueryable();

            if (!string.IsNullOrEmpty(search) && int.TryParse(search, out int accountId))
            {
                query = query.Where(t => t.AccountId == accountId);
            }

            var sortExpressions = new Dictionary<string, Expression<Func<Transaction, object>>>
            {
                { "AccountId", t => t.AccountId },
                { "DateOfTransaction", t => t.Date },
                { "Operation", t => t.Operation },
                { "Amount", t => t.Amount },
                { "Balance", t => t.Balance }
            };

            query = _sortingService.Sort(query, sortColumn, sortOrder, sortExpressions);

            Transactions = _paginationService.GetPage(query, CurrentPage, TransactionsPerPage)
                .Select(t => new TransactionViewModel
                {
                    AccountId = t.AccountId,
                    DateOfTransaction = t.Date,
                    Operation = t.Operation,
                    Amount = t.Amount,
                    Balance = t.Balance
                })
                .ToList();
        }
    }
}

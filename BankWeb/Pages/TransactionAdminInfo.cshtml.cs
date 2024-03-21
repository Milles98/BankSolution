using BankWeb.Data;
using BankWeb.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq.Expressions;

namespace BankWeb.Pages
{
    public class TransactionAdminInfoModel(BankAppData2Context context, ISortingService<Transaction> sortingService) : PageModel
    {
        private readonly BankAppData2Context _context = context;
        private readonly ISortingService<Transaction> _sortingService = sortingService;
        public List<TransactionViewModel> Transactions { get; set; } = new();

        public int CurrentPage { get; set; } = 1;
        public int TransactionsPerPage { get; set; } = 15;
        public int TotalPages => (int)Math.Ceiling(_context.Transactions.Count() / (double)TransactionsPerPage);
        public void OnGet(string sortColumn, string sortOrder)
        {
            if (Request.Query.ContainsKey("page"))
            {
                CurrentPage = int.Parse(Request.Query["page"]);
            }

            var query = _context.Transactions.AsQueryable();

            var sortExpressions = new Dictionary<string, Expression<Func<Transaction, object>>>
            {
                { "AccountId", t => t.AccountId },
                { "DateOfTransaction", t => t.Date },
                { "Operation", t => t.Operation },
                { "Amount", t => t.Amount },
                { "Balance", t => t.Balance }
            };

            query = _sortingService.Sort(query, sortColumn, sortOrder, sortExpressions);

            Transactions = query
                .Skip((CurrentPage - 1) * TransactionsPerPage)
                .Take(TransactionsPerPage)
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

    public class TransactionViewModel
    {
        public int AccountId { get; set; }
        public DateOnly DateOfTransaction { get; set; }
        public string Operation { get; set; }
        public decimal Amount { get; set; }
        public decimal Balance { get; set; }

    }
}

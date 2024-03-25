using BankWeb.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BankWeb.Pages
{
    public class IndexModel : PageModel
    {
        private readonly BankAppData2Context _context;

        public IndexModel(BankAppData2Context context)
        {
            _context = context;
        }

        public Dictionary<string, (int customers, int accounts, decimal totalBalance)> DataPerCountry { get; set; }

        public void OnGet()
        {
            DataPerCountry = _context.Customers
                .Include(c => c.Dispositions)
                .ThenInclude(d => d.Account)
                .GroupBy(c => c.Country)
                .ToDictionary(
                    g => g.Key,
                    g => (
                        customers: g.Count(),
                        accounts: g.SelectMany(c => c.Dispositions).Count(),
                        totalBalance: g.SelectMany(c => c.Dispositions.Select(d => d.Account.Balance)).Sum()
                    )
                );
        }
    }
}

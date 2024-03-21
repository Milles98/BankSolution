using BankWeb.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BankWeb.Pages
{
    public class IndexModel(BankAppData2Context context) : PageModel
    {
        private readonly BankAppData2Context _context = context;

        public int TotalCustomers { get; set; }
        public decimal TotalBalance { get; set; }
        public int TotalAccounts { get; set; }

        public void OnGet()
        {
            TotalCustomers = _context.Customers.Count();
            TotalBalance = _context.Accounts.Sum(a => a.Balance);
            TotalAccounts = _context.Accounts.Count();
        }
    }
}

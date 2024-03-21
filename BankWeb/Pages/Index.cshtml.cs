using BankWeb.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BankWeb.Pages
{
    public class IndexModel(BankAppData2Context context) : PageModel
    {
        private readonly BankAppData2Context _context = context;

        public int TotalCustomers { get; set; }
        public Dictionary<string, int> CustomersPerCountry { get; set; }

        public void OnGet()
        {
            TotalCustomers = _context.Customers.Count();
            CustomersPerCountry = _context.Customers
                .GroupBy(c => c.Country)
                .ToDictionary(g => g.Key, g => g.Count());
        }
    }
}

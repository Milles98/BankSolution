using BankWeb.Data;
using BankWeb.ViewModel;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace BankWeb.Pages
{
    public class SearchResultsModel : PageModel
    {
        private readonly BankAppData2Context _context;

        public SearchResultsModel(BankAppData2Context context)
        {
            _context = context;
        }

        public List<CustomerAccountViewModel> Customers { get; set; }

        public async Task OnGetAsync(string query)
        {
            Customers = await _context.Customers
                .Where(c => c.Givenname.Contains(query) || c.Surname.Contains(query) || c.City.Contains(query))
                .Select(c => new CustomerAccountViewModel
                {
                    CustomerId = c.CustomerId,
                    Givenname = c.Givenname,
                    Surname = c.Surname,
                    Streetaddress = c.Streetaddress,
                    City = c.City
                })
                .Take(50)
                .ToListAsync();
        }
    }
}

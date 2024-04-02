using DataLibrary.Data;
using DataLibrary.ViewModels;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace BankWeb.Pages
{
    public class SearchResultsModel : PageModel
    {
        private readonly BankAppDataContext _context;

        public SearchResultsModel(BankAppDataContext context)
        {
            _context = context;
        }

        public List<CustomerAccountViewModel> Customers { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }

        public async Task OnGetAsync(string query, int page = 1)
        {
            query = query.ToLower();

            int id;
            bool isNumeric = int.TryParse(query, out id);

            var customers = _context.Customers
                .Where(c => (isNumeric && c.CustomerId == id) ||
                c.Givenname.ToLower().Contains(query) ||
                c.Surname.ToLower().Contains(query) ||
                c.City.ToLower().Contains(query));

            var totalCustomers = await customers.CountAsync();
            TotalPages = (int)Math.Ceiling(totalCustomers / 50.0);

            Customers = await customers
                .OrderBy(c => c.CustomerId)
                .Skip((page - 1) * 50)
                .Take(50)
                .Select(c => new CustomerAccountViewModel
                {
                    CustomerId = c.CustomerId,
                    Givenname = c.Givenname,
                    Surname = c.Surname,
                    Streetaddress = c.Streetaddress,
                    City = c.City
                })
                .ToListAsync();

            ViewData["TotalPages"] = TotalPages;
            ViewData["CurrentPage"] = page;
        }


    }
}

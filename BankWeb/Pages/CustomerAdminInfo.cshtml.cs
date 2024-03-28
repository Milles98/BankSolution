using DataLibrary.Data;
using DataLibrary.Services.Interfaces;
using DataLibrary.ViewModels;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BankWeb.Pages
{
    public class CustomerAdminInfoModel : PageModel
    {
        private readonly BankAppData2Context _context;
        private readonly IPaginationService<Customer> _paginationService;

        public CustomerAdminInfoModel(BankAppData2Context context, IPaginationService<Customer> paginationService)
        {
            _context = context;
            _paginationService = paginationService;
        }

        public List<CustomerViewModel> Customers { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int CustomerPerPage { get; set; } = 7;
        public int TotalPages => (int)Math.Ceiling(_context.Customers.Count() / (double)CustomerPerPage);

        public void OnGet(string search)
        {
            if (Request.Query.ContainsKey("page"))
            {
                CurrentPage = int.Parse(Request.Query["page"]);
            }

            var query = _context.Customers.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                if (int.TryParse(search, out int customerId))
                {
                    query = query.Where(c => c.CustomerId == customerId);
                }
            }

            Customers = _paginationService
                .GetPage(query
                .Include(c => c.Dispositions)
                .ThenInclude(d => d.Account), CurrentPage, CustomerPerPage)
                .Select(c => new CustomerViewModel
                {
                    CustomerId = c.CustomerId,
                    AccountId = c.Dispositions.Select(d => d.AccountId).FirstOrDefault(),
                    Givenname = c.Givenname,
                    Surname = c.Surname,
                    Streetaddress = c.Streetaddress,
                    City = c.City,
                    Accounts = c.Dispositions.Select(d => d.Account).ToList()
                }).ToList();
        }
    }
}

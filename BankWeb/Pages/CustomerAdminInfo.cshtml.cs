using BankWeb.Data;
using BankWeb.Services.Interfaces;
using BankWeb.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BankWeb.Pages
{
    [Authorize(Roles = "Admin")]
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

            Customers = _paginationService.GetPage(query, CurrentPage, CustomerPerPage)
                .Select(c => new CustomerViewModel
                {
                    CustomerId = c.CustomerId,
                    Givenname = c.Givenname,
                    Surname = c.Surname,
                    Streetaddress = c.Streetaddress,
                    City = c.City
                }).ToList();
        }
    }
}

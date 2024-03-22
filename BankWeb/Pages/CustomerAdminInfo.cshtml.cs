using BankWeb.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BankWeb.Pages
{
    [Authorize(Roles = "Admin")]
    public class CustomerAdminInfoModel(BankAppData2Context context) : PageModel
    {
        private readonly BankAppData2Context _context = context;

        public List<CustomerViewModel> Customers { get; set; } = new();
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

            Customers = query
                .OrderBy(c => c.CustomerId)
                .Skip((CurrentPage - 1) * CustomerPerPage)
                .Take(CustomerPerPage)
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

    public class CustomerViewModel
    {
        public int CustomerId { get; set; }
        public string Givenname { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string Streetaddress { get; set; } = null!;
        public string City { get; set; } = null!;
    }
}

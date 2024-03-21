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
        public int CustomerPerPage { get; set; } = 15;
        public int TotalPages => (int)Math.Ceiling(_context.Customers.Count() / (double)CustomerPerPage);

        public void OnGet()
        {
            if (Request.Query.ContainsKey("page"))
            {
                CurrentPage = int.Parse(Request.Query["page"]);
            }

            Customers = _context.Customers
                .OrderBy(c => c.Givenname)
                .Skip((CurrentPage - 1) * CustomerPerPage)
                .Take(CustomerPerPage)
                .Select(c => new CustomerViewModel
                {
                    Givenname = c.Givenname,
                    Surname = c.Surname,
                    Streetaddress = c.Streetaddress,
                    City = c.City
                }).ToList();
        }
    }

    public class CustomerViewModel
    {
        public string Givenname { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string Streetaddress { get; set; } = null!;
        public string City { get; set; } = null!;
    }
}

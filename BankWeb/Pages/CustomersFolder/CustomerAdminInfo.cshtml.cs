using DataLibrary.Data;
using DataLibrary.Services;
using DataLibrary.Services.Interfaces;
using DataLibrary.ViewModels;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BankWeb.Pages.CustomersFolder
{
    public class CustomerAdminInfoModel : PageModel
    {
        private readonly ICustomerService _customerService;
        private readonly BankAppData2Context _context;

        public CustomerAdminInfoModel(ICustomerService customerService, BankAppData2Context context)
        {
            _customerService = customerService;
            _context = context;
        }

        public List<CustomerViewModel> Customers { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int CustomerPerPage { get; set; } = 7;
        public int TotalPages => _customerService.GetTotalPages(CustomerPerPage);

        public async Task OnGet(string search)
        {
            if (Request.Query.ContainsKey("page"))
            {
                CurrentPage = int.Parse(Request.Query["page"]);
            }

            Customers = await _customerService.GetCustomers(CurrentPage, CustomerPerPage, search);
        }

    }
}

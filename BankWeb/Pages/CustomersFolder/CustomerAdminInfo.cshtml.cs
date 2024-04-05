using DataLibrary.Data;
using DataLibrary.Services;
using DataLibrary.Services.Interfaces;
using DataLibrary.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BankWeb.Pages.CustomersFolder
{
    [Authorize(Roles = "Cashier")]
    public class CustomerAdminInfoModel : PageModel
    {
        private readonly ICustomerService _customerService;

        public CustomerAdminInfoModel(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        public List<CustomerViewModel> Customers { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int CustomerPerPage { get; set; } = 5;
        public int TotalPages => _customerService.GetTotalPages(CustomerPerPage);
        public int PageCount { get; set; }
        public string Search { get; set; }

        public async Task OnGet(string sortColumn, string sortOrder, string query)
        {
            Search = query;
            if (Request.Query.ContainsKey("page"))
            {
                CurrentPage = int.Parse(Request.Query["page"]);
            }

            var result = await _customerService.GetCustomers(CurrentPage, CustomerPerPage, sortColumn, sortOrder, Search);
            Customers = result.Item1;
            PageCount = (int)Math.Ceiling(result.Item2 / (double)CustomerPerPage);
        }


    }
}

using DataLibrary.Data;
using DataLibrary.Infrastructure.Paging;
using DataLibrary.Services;
using DataLibrary.Services.Interfaces;
using DataLibrary.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BankWeb.Pages.Customers
{
    // [Authorize(Roles = "Cashier")]
    public class CustomerAdminInfoModel(ICustomerService customerService) : PageModel
    {
        public PagedResult<CustomerViewModel> Customers { get; set; }
        public int CurrentPage { get; set; }
        public int CustomerPerPage { get; set; }
        public int TotalCustomers { get; set; }
        public string Search { get; set; }

        public async Task OnGet(string sortColumn, string sortOrder, int pageNo, string query)
        {
            Search = query;

            if (pageNo == 0)
                pageNo = 1;
            CurrentPage = pageNo;

            if (Request.Query.ContainsKey("page"))
            {
                CurrentPage = int.Parse(Request.Query["page"]);
            }

            TotalCustomers = customerService.GetTotalCustomers();

            Customers = await customerService.GetCustomers(CurrentPage, CustomerPerPage, sortColumn, sortOrder, Search);
        }
    }
}

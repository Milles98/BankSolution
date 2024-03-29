using DataLibrary.Data;
using DataLibrary.Services;
using DataLibrary.Services.Interfaces;
using DataLibrary.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BankWeb.Pages.CustomersFolder
{
    public class CustomerDetailsModel : PageModel
    {
        private readonly ICustomerService _customerService;

        public CustomerDetailsModel(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        public CustomerAccountViewModel Customer { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Customer = await _customerService.GetCustomerDetails(id);

            if (Customer == null)
            {
                return NotFound();
            }

            return Page();
        }
    }

}

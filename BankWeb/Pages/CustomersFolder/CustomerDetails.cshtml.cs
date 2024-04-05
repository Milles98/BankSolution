using DataLibrary.Data;
using DataLibrary.Services;
using DataLibrary.Services.Interfaces;
using DataLibrary.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BankWeb.Pages.CustomersFolder
{
    [Authorize(Roles = "Cashier")]
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

            // Assuming Gender is now included in Customer
            ViewData["GenderIcon"] = Customer.Gender == "Male" ? "fas fa-male" : "fas fa-female";

            return Page();
        }
    }

}

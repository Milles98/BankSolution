using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DataLibrary.Data;
using Microsoft.AspNetCore.Authorization;
using DataLibrary.Services.Interfaces;

namespace BankWeb.Pages.CustomerCRUD
{
    [Authorize(Roles = "Cashier")]
    public class DeleteModel : PageModel
    {
        private readonly IPersonService _personService;

        public DeleteModel(IPersonService personService)
        {
            _personService = personService;
        }

        [BindProperty]
        public Customer Customer { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Customer = await _personService.GetCustomerAsync(id.Value);

            if (Customer == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            return RedirectToPage("/CustomersFolder/DeleteAccessDenied");

            await _personService.DeleteCustomerAsync(id.Value);

            TempData["Message"] = $"Customer ID {id} has been permanently deleted at {DateTime.Now:yyyy-MM-dd}";

            return RedirectToPage("/CustomersFolder/CustomerAdminInfo");
        }



    }
}

using DataLibrary.Data;
using DataLibrary.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BankWeb.Pages.Customers
{
    [Authorize(Roles = "Cashier")]
    public class DeleteModel(IPersonService personService) : PageModel
    {
        [BindProperty]
        public Customer Customer { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Customer = await personService.GetCustomerAsync(id.Value);

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

            return RedirectToPage("/Customers/DeleteAccessDenied");

            await personService.DeleteCustomerAsync(id.Value);

            TempData["Message"] = $"Customer ID {id} has been permanently deleted at {DateTime.Now:yyyy-MM-dd}";

            return RedirectToPage("/Customers/CustomerAdminInfo");
        }



    }
}

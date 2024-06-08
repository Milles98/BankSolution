using DataLibrary.Services.Interfaces;
using DataLibrary.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BankWeb.Pages.Accounts
{
    // [Authorize(Roles = "Cashier")]
    public class CreateAccountModel(
        IAccountService accountService,
        ICustomerService customerService,
        ILogger<CreateAccountModel> logger)
        : PageModel
    {
        [BindProperty]
        public AccountViewModel Account { get; set; }

        public async Task<IActionResult> OnGetAsync(int customerId)
        {
            var customer = await customerService.GetCustomerDetails(customerId);
            if (customer == null)
            {
                return NotFound();
            }

            Account = new AccountViewModel { CustomerId = customerId };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        logger.LogError(error.ErrorMessage);
                    }
                }

                TempData["Message"] = "Failed to create account. Please check your input and try again.";
                TempData["MessageClass"] = "alert-danger";
                return Page();
            }

            Account.Created = DateTime.Now.ToString("yyyy-MM-dd");

            await accountService.CreateAccount(Account, Account.CustomerId);

            TempData["Message"] = "Account created successfully.";
            TempData["MessageClass"] = "alert-success";

            return RedirectToPage("/Customers/CustomerDetails", new { id = Account.CustomerId });
        }


    }

}

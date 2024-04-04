using DataLibrary.Services.Interfaces;
using DataLibrary.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BankWeb.Pages.AccountsFolder
{
    [Authorize(Roles = "Admin")]
    public class CreateAccountModel : PageModel
    {
        private readonly IAccountService _accountService;
        private readonly ICustomerService _customerService;
        private readonly ILogger<CreateAccountModel> _logger;

        public CreateAccountModel(IAccountService accountService, ICustomerService customerService, ILogger<CreateAccountModel> logger)
        {
            _accountService = accountService;
            _customerService = customerService;
            _logger = logger;
        }

        [BindProperty]
        public AccountViewModel Account { get; set; }

        public async Task<IActionResult> OnGetAsync(int customerId)
        {
            var customer = await _customerService.GetCustomerDetails(customerId);
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
                        _logger.LogError(error.ErrorMessage);
                    }
                }

                TempData["Message"] = "Failed to create account. Please check your input and try again.";
                TempData["MessageClass"] = "alert-danger";
                return Page();
            }

            Account.Created = DateTime.Now.ToString("yyyy-MM-dd");

            await _accountService.CreateAccount(Account, Account.CustomerId);

            TempData["Message"] = "Account created successfully.";
            TempData["MessageClass"] = "alert-success";

            return RedirectToPage("/CustomersFolder/CustomerDetails", new { id = Account.CustomerId });
        }


    }

}

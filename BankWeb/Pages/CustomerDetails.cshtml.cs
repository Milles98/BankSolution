using BankWeb.Data;
using BankWeb.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BankWeb.Pages
{
    public class CustomerDetailsModel : PageModel
    {
        private readonly BankAppData2Context _context;

        public CustomerDetailsModel(BankAppData2Context context)
        {
            _context = context;
        }

        public CustomerAccountViewModel Customer { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var customer = await _context.Customers
                .Include(c => c.Dispositions)
                .ThenInclude(d => d.Account)
                .FirstOrDefaultAsync(m => m.CustomerId == id);

            if (customer == null)
            {
                return NotFound();
            }

            Customer = new CustomerAccountViewModel
            {
                CustomerId = customer.CustomerId,
                Givenname = customer.Givenname,
                Surname = customer.Surname,
                Streetaddress = customer.Streetaddress,
                City = customer.City,
                Accounts = customer.Dispositions.Select(d => new AccountViewModel
                {
                    AccountId = d.Account.AccountId.ToString(),
                    Balance = d.Account.Balance,
                    Type = d.Type
                }).ToList(),
                TotalBalance = customer.Dispositions.Sum(d => d.Account.Balance)
            };

            return Page();
        }
    }
}

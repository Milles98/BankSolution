using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using DataLibrary.Data;

namespace BankWeb.Pages.CustomerCRUD
{
    public class CreateModel : PageModel
    {
        private readonly DataLibrary.Data.BankAppData2Context _context;

        public CreateModel(DataLibrary.Data.BankAppData2Context context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Customer Customer { get; set; } = default!;
        [BindProperty]
        public int BirthdayYear { get; set; }

        [BindProperty]
        public int BirthdayMonth { get; set; }

        [BindProperty]
        public int BirthdayDay { get; set; }
        [BindProperty]
        public string DispositionType { get; set; } = "OWNER";
        [BindProperty]
        public string Frequency { get; set; } = "Monthly";




        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (BirthdayYear > 0 && BirthdayMonth > 0 && BirthdayDay > 0)
            {
                Customer.Birthday = new DateOnly(BirthdayYear, BirthdayMonth, BirthdayDay);
            }

            // Create a new account for the customer
            var account = new Account
            {
                // Set properties for the account
                Frequency = Frequency, // Set this to whatever is appropriate
                Created = DateOnly.FromDateTime(DateTime.Today),
                Balance = 0, // Initial balance is 0
                             // You can add other properties as needed
            };

            // Create a new disposition for the customer and account
            var disposition = new Disposition
            {
                Customer = Customer,
                Account = account,
                Type = DispositionType // Set this to whatever is appropriate
            };

            // Add the disposition to the account's dispositions
            account.Dispositions.Add(disposition);

            // Add the account to the context so it gets saved to the database
            _context.Accounts.Add(account);

            _context.Customers.Add(Customer);
            await _context.SaveChangesAsync();

            return RedirectToPage("/CustomerDetails", new { id = Customer.CustomerId });
        }

    }
}

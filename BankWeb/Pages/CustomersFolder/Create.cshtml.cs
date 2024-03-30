using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using DataLibrary.Data;
using Microsoft.AspNetCore.Authorization;

namespace BankWeb.Pages.CustomerCRUD
{
    [Authorize(Roles = "Admin")]
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

            var account = new Account
            {
                Frequency = Frequency,
                Created = DateOnly.FromDateTime(DateTime.Today),
                Balance = 0,
            };

            var disposition = new Disposition
            {
                Customer = Customer,
                Account = account,
                Type = DispositionType
            };

            account.Dispositions.Add(disposition);

            _context.Accounts.Add(account);

            _context.Customers.Add(Customer);
            await _context.SaveChangesAsync();

            return RedirectToPage("/CustomerDetails", new { id = Customer.CustomerId });
        }

    }
}

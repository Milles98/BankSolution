using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DataLibrary.Data;
using Microsoft.AspNetCore.Authorization;

namespace BankWeb.Pages.CustomerCRUD
{
    [Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        private readonly DataLibrary.Data.BankAppDataContext _context;

        public EditModel(DataLibrary.Data.BankAppDataContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Customer Customer { get; set; } = default!;
        [BindProperty]
        public int BirthdayYear { get; set; }
        [BindProperty]
        public int BirthdayMonth { get; set; }

        [BindProperty]
        public int BirthdayDay { get; set; }
        public string DispositionType { get; set; } = "OWNER";
        [BindProperty]
        public string Frequency { get; set; } = "Monthly";

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers.FirstAsync(m => m.CustomerId == id);
            if (customer == null)
            {
                return NotFound();
            }
            Customer = customer;

            if (Customer.Birthday.HasValue)
            {
                BirthdayYear = Customer.Birthday.Value.Year;
                BirthdayMonth = Customer.Birthday.Value.Month;
                BirthdayDay = Customer.Birthday.Value.Day;
            }

            return Page();
        }

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

            _context.Attach(Customer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(Customer.CustomerId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            TempData["Message"] = "Customer details have been updated.";


            return RedirectToPage("/CustomersFolder/CustomerDetails", new { id = Customer.CustomerId });
        }


        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.CustomerId == id);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DataLibrary.Data;
using Microsoft.AspNetCore.Authorization;

namespace BankWeb.Pages.CustomerCRUD
{
    [Authorize(Roles = "Admin")]
    public class DeleteModel : PageModel
    {
        private readonly DataLibrary.Data.BankAppDataContext _context;

        public DeleteModel(DataLibrary.Data.BankAppDataContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Customer Customer { get; set; } = default!;

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
            else
            {
                Customer = customer;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers.Include(c => c.Dispositions)
                                                   .ThenInclude(d => d.Account)
                                                   .FirstAsync(c => c.CustomerId == id);
            if (customer != null)
            {
                var accountId = customer.Dispositions.First().Account.AccountId;
                var dispositionId = customer.Dispositions.First().DispositionId;

                // Remove all related dispositions and their accounts
                foreach (var disposition in customer.Dispositions.ToList())
                {
                    _context.Dispositions.Remove(disposition);
                    _context.Accounts.Remove(disposition.Account);
                }

                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();

                TempData["Message"] = $"Customer ID {customer.CustomerId} has been permanently deleted along with Account ID {accountId} and Disposition ID {dispositionId} at {DateTime.Now:yyyy-MM-dd}";
            }

            return RedirectToPage("/CustomersFolder/CustomerAdminInfo");
        }



    }
}

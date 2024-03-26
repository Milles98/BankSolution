using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DataLibrary.Data;

namespace BankWeb.Pages.CustomerCRUD
{
    public class IndexModel : PageModel
    {
        private readonly DataLibrary.Data.BankAppData2Context _context;

        public IndexModel(DataLibrary.Data.BankAppData2Context context)
        {
            _context = context;
        }

        public IList<Customer> Customer { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Customer = await _context.Customers.ToListAsync();
        }
    }
}

using DataLibrary.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BankWeb.Pages
{
    public class LoansModel : PageModel
    {
        private readonly BankAppDataContext _context;
        public List<Loan> Loans { get; set; }
        public LoansModel(BankAppDataContext context)
        {
            _context = context;
        }
        public void OnGet()
        {
            Loans = _context.Loans.Take(10).ToList();
        }
    }
}

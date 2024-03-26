using DataLibrary.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BankWeb.Pages
{
    public class AccountModel : PageModel
    {
        private readonly BankAppData2Context _context;
        public AccountModel(BankAppData2Context context)
        {
            _context = context;
        }

        public List<Account> Accounts { get; set; }

        public void OnGet(int accountId)
        {
            Accounts = _context.Accounts.Where(a => a.AccountId == accountId).ToList();
        }
    }
}

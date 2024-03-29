using DataLibrary.Data;
using DataLibrary.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;

namespace BankWeb.Pages.AccountsFolder
{
    public class AccountModel : PageModel
    {
        private readonly BankAppData2Context _context;
        public AccountModel(BankAppData2Context context)
        {
            _context = context;
        }

        public List<AccountViewModel> Accounts { get; set; }

        public void OnGet(int accountId)
        {
            Accounts = _context.Accounts
                .Where(a => a.AccountId == accountId)
                .Select(a => new AccountViewModel
                {
                    AccountId = a.AccountId.ToString(),
                    Frequency = a.Frequency,
                    Created = a.Created.ToString("yyyy-MM-dd"),
                    Balance = a.Balance,
                    Type = a.GetType().Name
                })
                .ToList();
        }
    }
}

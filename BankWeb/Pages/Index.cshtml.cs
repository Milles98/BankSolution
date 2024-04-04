using DataLibrary.Data;
using DataLibrary.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BankWeb.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IAccountService _accountService;

        public IndexModel(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public Dictionary<string, (int customers, int accounts, decimal totalBalance)> DataPerCountry { get; set; }

        public void OnGet()
        {
            DataPerCountry = _accountService.GetDataPerCountry();
        }

    }
}

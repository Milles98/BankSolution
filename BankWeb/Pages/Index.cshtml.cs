using DataLibrary.Data;
using DataLibrary.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BankWeb.Pages
{
    public class IndexModel(IAccountService accountService) : PageModel
    {
        public Dictionary<string, (int customers, int accounts, decimal totalBalance)> DataPerCountry { get; set; }

        public void OnGet()
        {
            DataPerCountry = accountService.GetDataPerCountry();
        }

    }
}

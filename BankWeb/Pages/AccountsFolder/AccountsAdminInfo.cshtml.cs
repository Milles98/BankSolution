using DataLibrary.Services.Interfaces;
using DataLibrary.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BankWeb.Pages.AccountsFolder
{
    [Authorize(Roles = "Cashier")]
    public class AccountsAdminInfoModel(IAccountService accountService) : PageModel
    {
        public List<AccountViewModel> Accounts { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int AccountPerPage { get; set; } = 50;
        public int TotalPages => accountService.GetTotalPages(AccountPerPage);
        public int PageCount { get; set; }
        public string Search { get; set; }

        public async Task OnGet(string sortColumn, string sortOrder, string query)
        {
            Search = query;
            if (Request.Query.ContainsKey("page"))
            {
                CurrentPage = int.Parse(Request.Query["page"]);
            }

            var result = await accountService.GetAccounts(CurrentPage, AccountPerPage, sortColumn, sortOrder, Search);
            Accounts = result.Item1;
            PageCount = (int)Math.Ceiling(result.Item2 / (double)AccountPerPage);
        }
    }

}

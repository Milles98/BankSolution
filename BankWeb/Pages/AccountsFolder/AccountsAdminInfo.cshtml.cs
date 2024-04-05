using DataLibrary.Services.Interfaces;
using DataLibrary.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BankWeb.Pages.AccountsFolder
{
    [Authorize(Roles = "Cashier")]
    public class AccountsAdminInfoModel : PageModel
    {
        private readonly IAccountService _accountService;

        public AccountsAdminInfoModel(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public List<AccountViewModel> Accounts { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int AccountPerPage { get; set; } = 5;
        public int TotalPages => _accountService.GetTotalPages(AccountPerPage);
        public int PageCount { get; set; }
        public string Search { get; set; }

        public async Task OnGet(string sortColumn, string sortOrder, string query)
        {
            Search = query;
            if (Request.Query.ContainsKey("page"))
            {
                CurrentPage = int.Parse(Request.Query["page"]);
            }

            var result = await _accountService.GetAccounts(CurrentPage, AccountPerPage, sortColumn, sortOrder, Search);
            Accounts = result.Item1;
            PageCount = (int)Math.Ceiling(result.Item2 / (double)AccountPerPage);
        }
    }

}

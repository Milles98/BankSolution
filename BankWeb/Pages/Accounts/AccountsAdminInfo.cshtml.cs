using DataLibrary.Infrastructure.Paging;
using DataLibrary.Services.Interfaces;
using DataLibrary.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BankWeb.Pages.AccountsFolder
{
    // [Authorize(Roles = "Cashier")]
    public class AccountsAdminInfoModel(IAccountService accountService) : PageModel
    {
        public PagedResult<AccountViewModel> Accounts { get; set; }
        public int CurrentPage { get; set; }
        public int AccountPerPage { get; set; }
        public int TotalAccounts { get; set; }
        public string Search { get; set; }


        public async Task OnGet(string sortColumn, string sortOrder, int pageNo, string query)
        {
            Search = query;

            if (pageNo == 0)
                pageNo = 1;
            CurrentPage = pageNo;

            if (Request.Query.ContainsKey("page"))
            {
                CurrentPage = int.Parse(Request.Query["page"]);
            }

            TotalAccounts = accountService.GetTotalAccounts();

            Accounts = await accountService
                .GetAccounts(CurrentPage, AccountPerPage, sortColumn, sortOrder, query);
        }
    }

}

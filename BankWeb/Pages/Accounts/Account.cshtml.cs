using DataLibrary.Services.Interfaces;
using DataLibrary.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;

namespace BankWeb.Pages.AccountsFolder
{
    [Authorize(Roles = "Cashier")]
    public class AccountModel(IAccountService accountService) : PageModel
    {
        public List<AccountViewModel> Accounts { get; set; }

        public void OnGet(string accountIds)
        {
            var accountIdList = accountIds.Split(',').Select(int.Parse).ToList();
            Accounts = accountService.GetAccountDetails(accountIdList);
        }


    }
}

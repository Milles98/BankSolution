using DataLibrary.Services.Interfaces;
using DataLibrary.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;

namespace BankWeb.Pages.AccountsFolder
{
    public class AccountModel : PageModel
    {
        private readonly IAccountService _accountService;

        public AccountModel(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public List<AccountViewModel> Accounts { get; set; }

        public void OnGet(int accountId)
        {
            Accounts = _accountService.GetAccountDetails(accountId);
        }
    }
}

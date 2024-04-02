using DataLibrary.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLibrary.Services.Interfaces
{
    public interface IAccountService
    {
        List<AccountViewModel> GetAccountDetails(List<int> accountIds);
        Task CreateAccount(AccountViewModel accountViewModel, int customerId);
    }
}

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
        Dictionary<string, (int customers, int accounts, decimal totalBalance)> GetDataPerCountry();
        List<AccountViewModel> GetAccountDetails(List<int> accountIds);
        Task CreateAccount(AccountViewModel accountViewModel, int customerId);
        Task<(List<AccountViewModel>, int)> GetAccounts(int currentPage, int accountsPerPage, string sortColumn, string sortOrder, string search);
        int GetTotalPages(int accountsPerPage);
        Task DeleteAccountAndRelatedData(int id);
        int GetTotalAccounts();
    }
}

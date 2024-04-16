using DataLibrary.Infrastructure.Paging;
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
        Task<PagedResult<AccountViewModel>> GetAccounts(int pageNo, int accountsPerPage, string sortColumn, string sortOrder, string search);
        Task DeleteAccountAndRelatedData(int id);
        int GetTotalAccounts();
    }
}

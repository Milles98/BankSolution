using DataLibrary.Data;
using DataLibrary.Infrastructure.Paging;
using DataLibrary.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLibrary.Services.Interfaces
{
    public interface ITransactionService
    {
        BankAppDataContext GetDbContext();
        Task<PagedResult<TransactionViewModel>> GetTransactions(int currentPage, int transactionsPerPage, string sortColumn, string sortOrder, string search);
        Task<List<TransactionViewModel>> GetTransactionsForIndividualAccount(string sortColumn, string sortOrder, int accountId, string search);
        Task<TransactionViewModel> GetTransactionDetails(int transactionId);
        Task<List<TransactionViewModel>> GetTransactionsForAccount(int accountId);
        Task<decimal?> GetAccountBalance(int accountId);
        Task<(List<TransactionViewModel>, bool)> LoadMoreTransactions(int accountId, string loadedTransactionIds, int pageNo);
        int GetTotalTransactions();
    }
}

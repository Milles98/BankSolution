using DataLibrary.Data;
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
        Task<List<TransactionViewModel>> GetTransactions(int currentPage, int transactionsPerPage,
            string sortColumn, string sortOrder, string search);
        int GetTotalPages(int transactionsPerPage);
        Task<TransactionViewModel> GetTransactionDetails(int transactionId);
        Task<List<TransactionViewModel>> GetTransactionsForAccount(int accountId, DateTime? lastFetchedTransactionTimestamp);
        Task<decimal?> GetAccountBalance(int accountId);
        Task<(List<TransactionViewModel>, bool)> LoadMoreTransactions(int accountId, DateTime? lastFetchedTransactionTimestamp, string loadedTransactionIds);
    }
}

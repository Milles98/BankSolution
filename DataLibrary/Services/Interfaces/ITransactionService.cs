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
        Task<List<TransactionViewModel>> GetTransactions(int currentPage, int transactionsPerPage,
            string sortColumn, string sortOrder, string search);
        int GetTotalPages(int transactionsPerPage);
        Task<TransactionViewModel> GetTransactionDetails(int transactionId);
    }
}

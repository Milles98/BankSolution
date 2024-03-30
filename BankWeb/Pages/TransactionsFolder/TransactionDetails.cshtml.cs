using DataLibrary.Data;
using DataLibrary.Services.Interfaces;
using DataLibrary.ViewModels;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BankWeb.Pages.TransactionsFolder
{
    public class TransactionDetailsModel : PageModel
    {
        private readonly ITransactionService _transactionService;

        public TransactionDetailsModel(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }
        public TransactionViewModel Transaction { get; set; }

        public async Task OnGet(int transactionId)
        {
            Transaction = await _transactionService.GetTransactionDetails(transactionId);
        }

    }

}

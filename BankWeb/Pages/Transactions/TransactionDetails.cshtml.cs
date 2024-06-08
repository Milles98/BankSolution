using DataLibrary.Data;
using DataLibrary.Services.Interfaces;
using DataLibrary.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BankWeb.Pages.Transactions
{
    // [Authorize(Roles = "Cashier")]
    public class TransactionDetailsModel(ITransactionService transactionService) : PageModel
    {
        public TransactionViewModel Transaction { get; set; }

        public async Task OnGet(int transactionId)
        {
            Transaction = await transactionService.GetTransactionDetails(transactionId);
        }

    }

}

using DataLibrary.Data;
using DataLibrary.Services.Interfaces;
using DataLibrary.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace DataLibrary.Services
{
    public class AccountService : IAccountService
    {
        private readonly BankAppData2Context _context;

        public AccountService(BankAppData2Context context)
        {
            _context = context;
        }

        public List<AccountViewModel> GetAccountDetails(int accountId)
        {
            return _context.Accounts
                .Where(a => a.AccountId == accountId)
                .Include(a => a.Dispositions)
                .ThenInclude(d => d.Customer)
                .Select(a => new AccountViewModel
                {
                    AccountId = a.AccountId.ToString(),
                    Frequency = a.Frequency,
                    Created = a.Created.ToString("yyyy-MM-dd"),
                    Balance = a.Balance,
                    Type = a.GetType().Name,
                    Customers = a.Dispositions.Select(d => new CustomerDispositionViewModel
                    {
                        Customer = d.Customer,
                        DispositionType = d.Type
                    }).ToList()
                })
                .ToList();
        }

        public async Task CreateAccount(AccountViewModel accountViewModel, int customerId)
        {
            var account = new Account
            {
                Frequency = accountViewModel.Frequency,
                Created = DateOnly.Parse(accountViewModel.Created),
                Balance = accountViewModel.Balance
            };

            var customer = await _context.Customers.FindAsync(customerId);
            if (customer == null)
            {
                throw new Exception("Customer not found");
            }

            var disposition = new Disposition
            {
                Account = account,
                Customer = customer,
                Type = "Owner"
            };

            _context.Dispositions.Add(disposition);
            await _context.SaveChangesAsync();
        }

    }
}

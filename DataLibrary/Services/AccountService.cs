using AutoMapper;
using DataLibrary.Data;
using DataLibrary.Services.Interfaces;
using DataLibrary.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DataLibrary.Services
{
    public class AccountService(
        BankAppDataContext context,
        IPaginationService<Account> paginationService,
        ISortingService<Account> sortingService,
        IMapper mapper)
        : IAccountService
    {
        public Dictionary<string, (int customers, int accounts, decimal totalBalance)> GetDataPerCountry()
        {
            return context.Customers
                .Include(c => c.Dispositions)
                .ThenInclude(d => d.Account)
                .GroupBy(c => c.Country)
                .ToDictionary(
                    g => g.Key,
                    g => (
                        customers: g.Count(),
                        accounts: g.SelectMany(c => c.Dispositions.Select(d => d.AccountId)).Distinct().Count(),
                        totalBalance: g.SelectMany(c => c.Dispositions.Select(d => d.Account.Balance)).Sum()
                    )
                );
        }


        public List<AccountViewModel> GetAccountDetails(List<int> accountIds)
        {
            var accounts = context.Accounts
                .Where(a => accountIds.Contains(a.AccountId))
                .Include(a => a.Dispositions)
                .ThenInclude(d => d.Customer)
                .ToList();

            var accountViewModels = mapper.Map<List<AccountViewModel>>(accounts);

            return accountViewModels;
        }

        public async Task CreateAccount(AccountViewModel accountViewModel, int customerId)
        {
            var account = new Account
            {
                Frequency = accountViewModel.Frequency,
                Created = DateOnly.Parse(accountViewModel.Created),
                Balance = accountViewModel.Balance
            };

            var customer = await context.Customers.FindAsync(customerId);
            if (customer == null)
            {
                throw new Exception("Customer not found");
            }

            var disposition = new Disposition
            {
                Account = account,
                Customer = customer,
                Type = "OWNER"
            };

            context.Dispositions.Add(disposition);
            await context.SaveChangesAsync();
        }

        public async Task<(List<AccountViewModel>, int)> GetAccounts(int currentPage, int accountsPerPage, string sortColumn, string sortOrder, string search)
        {
            var query = context.Accounts.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                if (int.TryParse(search, out int accountId))
                {
                    query = query.Where(a => a.AccountId == accountId);
                }
                else
                {
                    query = query.Where(a => a.Frequency.ToLower().Contains(search));
                }
            }

            int totalAccounts = await query.CountAsync();

            var sortExpressions = new Dictionary<string, Expression<Func<Account, object>>>
            {
                { "AccountId", a => a.AccountId },
                { "Frequency", a => a.Frequency },
                { "Created", a => a.Created },
                { "Balance", a => a.Balance }
            };

            query = sortingService.Sort(query, sortColumn, sortOrder, sortExpressions);

            var accounts = await paginationService
            .GetPage(query
            .Include(a => a.Dispositions)
            .ThenInclude(d => d.Customer), currentPage, accountsPerPage)
            .ToListAsync();

            var accountViewModels = mapper.Map<List<AccountViewModel>>(accounts);

            return (accountViewModels, totalAccounts);
        }

        public int GetTotalPages(int accountsPerPage)
        {
            var query = context.Accounts.AsQueryable();
            return paginationService.GetTotalPages(query, accountsPerPage);
        }

        public async Task DeleteAccountAndRelatedData(int id)
        {
            // Find the account
            var account = await context.Accounts.FindAsync(id);
            if (account == null)
            {
                throw new Exception("Account not found");
            }

            // Find and remove all related dispositions
            var dispositions = context.Dispositions.Where(d => d.AccountId == id);
            context.Dispositions.RemoveRange(dispositions);

            // Find and remove all related transactions
            var transactions = context.Transactions.Where(t => t.AccountId == id);
            context.Transactions.RemoveRange(transactions);

            // Remove the account itself
            context.Accounts.Remove(account);

            // Save changes to the database
            await context.SaveChangesAsync();
        }


    }
}

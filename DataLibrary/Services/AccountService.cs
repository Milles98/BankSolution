using AutoMapper;
using DataLibrary.Data;
using DataLibrary.Infrastructure.Paging;
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

            var transaction = new Transaction
            {
                AccountId = account.AccountId,
                Amount = account.Balance,
                Balance = account.Balance,
                Date = DateOnly.FromDateTime(DateTime.Today),
                Type = "Credit",
                Operation = "Initial Deposit"
            };

            account.Transactions.Add(transaction);
            await context.SaveChangesAsync();
        }

        public async Task<PagedResult<AccountViewModel>> GetAccounts(int pageNo, int accountsPerPage, string sortColumn, string sortOrder, string search)
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

            var sortExpressions = new Dictionary<string, Expression<Func<Account, object>>>
                {
                    { "AccountId", a => a.AccountId },
                    { "Frequency", a => a.Frequency },
                    { "Created", a => a.Created },
                    { "Balance", a => a.Balance }
                };

            query = sortingService.Sort(query, sortColumn, sortOrder, sortExpressions);

            var accounts = await query
               .Include(a => a.Dispositions)
               .ThenInclude(d => d.Customer)
               .GetPaged(pageNo, 48);

            var accountViewModels = mapper.Map<List<AccountViewModel>>(accounts.Results);

            return new PagedResult<AccountViewModel>
            {
                CurrentPage = accounts.CurrentPage,
                PageCount = accounts.PageCount,
                PageSize = accounts.PageSize,
                RowCount = accounts.RowCount,
                Results = accountViewModels
            };
        }



        public async Task DeleteAccountAndRelatedData(int id)
        {
            var account = await context.Accounts.FindAsync(id);
            if (account == null)
            {
                throw new Exception("Account not found");
            }

            var dispositions = context.Dispositions.Where(d => d.AccountId == id);
            context.Dispositions.RemoveRange(dispositions);

            var transactions = context.Transactions.Where(t => t.AccountId == id);
            context.Transactions.RemoveRange(transactions);

            context.Accounts.Remove(account);

            await context.SaveChangesAsync();
        }

        public int GetTotalAccounts()
        {
            return context.Accounts.Count();
        }



    }
}

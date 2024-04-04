﻿using DataLibrary.Data;
using DataLibrary.Services.Interfaces;
using DataLibrary.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace DataLibrary.Services
{
    public class AccountService : IAccountService
    {
        private readonly BankAppDataContext _context;

        public AccountService(BankAppDataContext context)
        {
            _context = context;
        }

        public Dictionary<string, (int customers, int accounts, decimal totalBalance)> GetDataPerCountry()
        {
            return _context.Customers
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
            return _context.Accounts
                .Where(a => accountIds.Contains(a.AccountId))
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
                Type = "OWNER"
            };

            _context.Dispositions.Add(disposition);
            await _context.SaveChangesAsync();
        }

    }
}

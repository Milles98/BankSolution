﻿using DataLibrary.Data;
using DataLibrary.Services.Interfaces;
using DataLibrary.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataLibrary.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly BankAppDataContext _context;
        private readonly IPaginationService<Transaction> _paginationService;
        private readonly ISortingService<Transaction> _sortingService;

        public TransactionService(BankAppDataContext context, IPaginationService<Transaction> paginationService, ISortingService<Transaction> sortingService)
        {
            _context = context;
            _paginationService = paginationService;
            _sortingService = sortingService;
        }

        public async Task<List<TransactionViewModel>> GetTransactions(int currentPage, int transactionsPerPage, string sortColumn, string sortOrder, string search)
        {
            var query = _context.Transactions.AsQueryable();

            if (!string.IsNullOrEmpty(search) && int.TryParse(search, out int transactionId))
            {
                query = query.Where(t => t.TransactionId == transactionId);
            }

            var sortExpressions = new Dictionary<string, Expression<Func<Transaction, object>>>
            {
                { "TransactionId", t => t.TransactionId},
                { "AccountId", t => t.AccountId },
                { "CustomerId", t => t.AccountNavigation.Dispositions.FirstOrDefault().CustomerId },
                { "DateOfTransaction", t => t.Date },
                { "Operation", t => t.Operation },
                { "Amount", t => t.Amount },
                { "Balance", t => t.Balance }
            };

            query = _sortingService.Sort(query, sortColumn, sortOrder, sortExpressions);

            var transactions = await _paginationService.GetPage(query, currentPage, transactionsPerPage)
                .Select(t => new TransactionViewModel
                {
                    TransactionId = t.TransactionId,
                    AccountId = t.AccountId,
                    CustomerId = t.AccountNavigation.Dispositions.FirstOrDefault().CustomerId,
                    DateOfTransaction = t.Date,
                    Operation = t.Operation,
                    Amount = t.Amount,
                    Balance = t.Balance
                })
                .ToListAsync();

            return transactions;
        }

        public int GetTotalPages(int transactionsPerPage)
        {
            var query = _context.Transactions.AsQueryable();
            return _paginationService.GetTotalPages(query, transactionsPerPage);
        }

        public async Task<TransactionViewModel> GetTransactionDetails(int transactionId)
        {
            var transaction = await _context.Transactions.FirstAsync(t => t.TransactionId == transactionId);
            if (transaction != null)
            {
                return new TransactionViewModel
                {
                    TransactionId = transaction.TransactionId,
                    AccountId = transaction.AccountId,
                    Amount = transaction.Amount,
                    Balance = transaction.Balance,
                    DateOfTransaction = transaction.Date,
                    Operation = transaction.Operation
                };
            }

            return null;
        }

        public async Task<List<TransactionViewModel>> GetTransactionsForAccount(int accountId, DateTime? lastFetchedTransactionTimestamp)
        {
            var transactionsQuery = _context.Transactions
                .Where(t => t.AccountId == accountId);

            if (lastFetchedTransactionTimestamp.HasValue)
            {
                var lastFetchedDate = DateOnly.FromDateTime(lastFetchedTransactionTimestamp.Value);
                transactionsQuery = transactionsQuery
                    .Where(t => t.Date < lastFetchedDate);
            }


            var transactions = await transactionsQuery
                .OrderByDescending(t => t.Date)
                .Take(20)
                .Select(t => new TransactionViewModel
                {
                    TransactionId = t.TransactionId,
                    DateOfTransaction = t.Date,
                    Type = t.Type,
                    Operation = t.Operation,
                    Amount = t.Amount,
                    Balance = t.Balance
                }).ToListAsync();


            return transactions;
        }



        public async Task<decimal> GetAccountBalance(int accountId)
        {
            var account = await _context.Accounts.FirstAsync(a => a.AccountId == accountId);
            return account.Balance;
        }



    }
}

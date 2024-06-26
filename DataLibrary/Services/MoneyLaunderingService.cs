﻿using DataLibrary.Data;
using DataLibrary.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLibrary.Services
{
    public class MoneyLaunderingService : IMoneyLaunderingService
    {
        private const decimal SingleTransactionLimit = 15000m;
        private const decimal TotalTransactionLimit = 23000m;
        private const int DaysToCheck = 3;

        private readonly BankAppDataContext _context;
        public MoneyLaunderingService(BankAppDataContext context)
        {
            _context = context;
        }

        public async Task<List<Disposition>> GetDispositionsAsync(string country)
        {
            return await _context.Dispositions
                .Include(d => d.Account)
                .ThenInclude(a => a.Transactions)
                .Include(d => d.Customer)
                .Where(d => d.Customer.Country == country)
                .ToListAsync();
        }

        public async Task DetectAndReportSuspiciousActivity()
        {
            var countries = new List<string> { "Sweden", "Norway", "Denmark", "Finland" };

            var currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var lastRunTimeFilePath = Path.Combine(currentDirectory, "..", "..", "..", "SuspicionReport", "lastRunTime.txt");
            DateOnly globalLastRunTime = GetLastRunTime(lastRunTimeFilePath);

            DateOnly latestTransactionDateAcrossCountries = globalLastRunTime;

            foreach (var country in countries)
            {
                Console.WriteLine($"Starting detection for {country}. Last run time: {globalLastRunTime}");

                var dispositions = await GetDispositionsAsync(country);
                var (suspiciousUsers, latestTransactionDate) = DetectSuspiciousActivity(dispositions, globalLastRunTime);

                var reportFilePath = Path.Combine(currentDirectory, "..", "..", "..", "SuspicionReport", $"report_{country}.txt");
                GenerateReport(suspiciousUsers, reportFilePath, country);

                Console.WriteLine("--------------------------------------------\n");
                latestTransactionDateAcrossCountries = latestTransactionDate > latestTransactionDateAcrossCountries ? latestTransactionDate : latestTransactionDateAcrossCountries;
            }

            SaveLastRunTime(lastRunTimeFilePath);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Suspicious activity detection completed.");
            Console.ResetColor();
        }

        public (List<string>, DateOnly) DetectSuspiciousActivity(List<Disposition> dispositions, DateOnly lastRunDate)
        {
            var suspiciousUsers = new List<string>();
            DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
            DateOnly threeDaysAgo = today.AddDays(-DaysToCheck);

            int totalDispositions = dispositions.Count;
            int currentDisposition = 0;

            foreach (var disposition in dispositions)
            {
                currentDisposition++;
                Console.Write($"\rChecking customer {currentDisposition} of {totalDispositions} for suspicious activity...");

                var account = disposition.Account;
                var transactions = account.Transactions;

                CheckSingleTransactionLimit(transactions, disposition, suspiciousUsers);

                CheckTotalTransactionLimit(transactions, threeDaysAgo, today, disposition, suspiciousUsers);

                if (transactions.Any())
                {
                    var maxTransactionDate = transactions.Max(t => t.Date);
                    lastRunDate = maxTransactionDate > lastRunDate ? maxTransactionDate : lastRunDate;
                }
            }

            Console.WriteLine($"\nCompleted checking all customers for suspicious activity.");
            return (suspiciousUsers, lastRunDate);
        }

        private void CheckSingleTransactionLimit(IEnumerable<Transaction> transactions, Disposition disposition, List<string> suspiciousUsers)
        {
            foreach (var transaction in transactions)
            {
                if (Math.Abs(transaction.Amount) > SingleTransactionLimit)
                {
                    suspiciousUsers.Add($"{disposition.Customer.Givenname} {disposition.Customer.Surname}, Account ID: {disposition.Account.AccountId}, Transaction ID: {transaction.TransactionId}, Amount: {transaction.Amount} SEK, 'Single transaction exceeds limit'");
                }
            }
        }

        private void CheckTotalTransactionLimit(IEnumerable<Transaction> transactions, DateOnly threeDaysAgo, DateOnly today, Disposition disposition, List<string> suspiciousUsers)
        {
            var recentTransactions = transactions.Where(t => t.Date >= threeDaysAgo && t.Date <= today);
            var totalAmountLastThreeDays = recentTransactions.Sum(t => t.Amount);

            if (Math.Abs(totalAmountLastThreeDays) > TotalTransactionLimit)
            {
                suspiciousUsers.Add($"{disposition.Customer.Givenname} {disposition.Customer.Surname}, Account ID: {disposition.Account.AccountId}, Total Amount in Last Three Days: {totalAmountLastThreeDays} SEK, 'Total transactions in last three days exceed limit'");
            }
        }


        public void GenerateReport(List<string> suspiciousUsers, string filePath, string country)
        {
            var directoryPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            HashSet<string> existingEntries = new HashSet<string>();
            if (File.Exists(filePath))
            {
                existingEntries = new HashSet<string>(File.ReadAllLines(filePath));
            }

            List<string> newEntries = new List<string>();
            foreach (var user in suspiciousUsers)
            {
                if (!existingEntries.Contains(user))
                {
                    newEntries.Add(user);
                }
            }

            File.AppendAllLines(filePath, newEntries);
            if (newEntries.Count > 0)
            {
                File.AppendAllLines(filePath, newEntries);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nReport for {country} generated with {newEntries.Count} new suspicious transactions.");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\nNo new suspicious transactions for {country}.");
                Console.ResetColor();
            }
        }

        public void SaveLastRunTime(string filePath)
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.UtcNow);
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            File.WriteAllText(filePath, today.ToString() + Environment.NewLine);
        }

        public DateOnly GetLastRunTime(string filePath)
        {
            if (!File.Exists(filePath) || new FileInfo(filePath).Length == 0)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                File.WriteAllText(filePath, DateOnly.MinValue.ToString() + Environment.NewLine);
                return DateOnly.MinValue;
            }

            var allRunTimes = File.ReadAllLines(filePath);
            var lastRunTimeStr = allRunTimes.Last();
            return DateOnly.Parse(lastRunTimeStr);
        }

    }
}

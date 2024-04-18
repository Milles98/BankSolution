using DataLibrary.Data;
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

                Console.WriteLine($"Detected {suspiciousUsers.Count} suspicious users for {country}.\n");
                Console.WriteLine("--------------------------------------------\n");
                latestTransactionDateAcrossCountries = latestTransactionDate > latestTransactionDateAcrossCountries ? latestTransactionDate : latestTransactionDateAcrossCountries;
            }

            SaveLastRunTime(lastRunTimeFilePath);

            Console.WriteLine("Suspicious activity detection completed.");
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
                var date = lastRunDate;
                var transactions = account.Transactions.Where(t => t.Date > date && t.Date <= today);

                foreach (var transaction in transactions)
                {
                    if (Math.Abs(transaction.Amount) > SingleTransactionLimit)
                    {
                        suspiciousUsers.Add($"{disposition.Customer.Givenname} {disposition.Customer.Surname}, Account ID: {account.AccountId}, Transaction ID: {transaction.TransactionId}, 'Single transaction exceeds limit'");
                    }
                }

                var recentTransactions = account.Transactions.Where(t => t.Date >= threeDaysAgo && t.Date <= today);
                var totalAmountLastThreeDays = recentTransactions.Sum(t => t.Amount);

                if (Math.Abs(totalAmountLastThreeDays) > TotalTransactionLimit)
                {
                    suspiciousUsers.Add($"{disposition.Customer.Givenname} {disposition.Customer.Surname}, Account ID: {account.AccountId}, 'Total transactions in last three days exceed limit'");
                }

                if (transactions.Any())
                {
                    var maxTransactionDate = transactions.Max(t => t.Date);
                    lastRunDate = maxTransactionDate > lastRunDate ? maxTransactionDate : lastRunDate;
                }
            }

            Console.WriteLine($"\nCompleted checking all customers for suspicious activity.");
            return (suspiciousUsers, lastRunDate);
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
                File.AppendAllText(filePath, "\n---\n");
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
            Console.WriteLine($"\nReport for {country} generated with {newEntries.Count} new suspicious users.");
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

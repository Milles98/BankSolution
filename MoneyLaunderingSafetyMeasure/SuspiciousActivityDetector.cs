using DataLibrary.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MoneyLaunderingSafetyMeasure
{
    public class SuspiciousActivityDetector
    {
        private const decimal SingleTransactionLimit = 15000m;
        private const decimal TotalTransactionLimit = 23000m;
        private const int DaysToCheck = 3;

        public async Task<List<Disposition>> GetDispositionsAsync(BankAppDataContext dbContext, string country)
        {
            return await dbContext.Dispositions
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


        public void SaveLastRunTime(DateOnly lastRunDate, string filePath)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            File.WriteAllText(filePath, lastRunDate.ToString() + Environment.NewLine);
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

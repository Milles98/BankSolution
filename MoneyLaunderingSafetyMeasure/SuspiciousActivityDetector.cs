using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DataLibrary.Data;
using Microsoft.EntityFrameworkCore; // Assuming your user and transaction models are in this namespace

namespace MoneyLaunderingSafetyMeasure
{
    public class SuspiciousActivityDetector
    {
        private const decimal SingleTransactionLimit = 15000m;
        private const decimal TotalTransactionLimit = 23000m;
        private const int TransactionPeriodInHours = 72;

        public List<Disposition> GetDispositions(BankAppData2Context dbContext, string country)
        {
            return dbContext.Dispositions
                .Include(d => d.Account)
                .ThenInclude(a => a.Transactions)
                .Include(d => d.Customer)
                .Where(d => d.Customer.Country == country)
                .ToList();
        }


        public List<string> DetectSuspiciousActivity(List<Disposition> dispositions, DateTime lastRunTime)
        {
            var suspiciousUsers = new List<string>();

            foreach (var disposition in dispositions)
            {
                var account = disposition.Account;

                var newTransactions = account.Transactions
                    .Where(t => t.Date >= DateOnly.FromDateTime(lastRunTime));

                if (newTransactions.Any(t => t.Amount > SingleTransactionLimit))
                {
                    suspiciousUsers.Add($"{disposition.Customer.Givenname} {disposition.Customer.Surname}, {account.AccountId}");
                    continue;
                }

                var recentTransactions = newTransactions
                    .Where(t => t.Date >= DateOnly.FromDateTime(DateTime.Now.AddDays(-3)));

                if (recentTransactions.Sum(t => t.Amount) > TotalTransactionLimit)
                {
                    suspiciousUsers.Add($"{disposition.Customer.Givenname} {disposition.Customer.Surname}, {account.AccountId}");
                }
            }

            return suspiciousUsers;
        }


        public void GenerateReport(List<string> suspiciousUsers, string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                }
                else
                {
                    File.AppendAllText(filePath, "\n---\n");
                }

                File.AppendAllLines(filePath, suspiciousUsers);
                Console.WriteLine($"Report generated at {filePath} with {suspiciousUsers.Count} suspicious users.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while generating the report: {ex.Message}");
            }
        }



        public void SaveLastRunTime(DateTime lastRunTime, string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                }

                File.WriteAllText(filePath, lastRunTime.ToString());
                Console.WriteLine($"Last run time saved at {filePath}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while saving the last run time: {ex.Message}");
            }
        }


        public DateTime GetLastRunTime(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                    File.WriteAllText(filePath, DateTime.MinValue.ToString());
                }

                var lastRunTimeStr = File.ReadAllText(filePath);
                return DateTime.Parse(lastRunTimeStr);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while getting the last run time: {ex.Message}");
                return DateTime.MinValue;
            }
        }
    }
}

using DataLibrary.Data;
using Microsoft.EntityFrameworkCore;
using MoneyLaunderingSafetyMeasure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<BankAppDataContext>();
        optionsBuilder.UseSqlServer("Server=localhost;Database=BankAppData;Trusted_Connection=True;TrustServerCertificate=true;MultipleActiveResultSets=true");

        try
        {
            using var dbContext = new BankAppDataContext(optionsBuilder.Options);
            var detector = new SuspiciousActivityDetector();

            var countries = new List<string> { "Sweden", "Norway", "Denmark", "Finland" };

            var currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var lastRunTimeFilePath = Path.Combine(currentDirectory, "..", "..", "..", "SuspicionReport", "lastRunTime.txt");
            DateOnly globalLastRunTime = detector.GetLastRunTime(lastRunTimeFilePath);

            DateOnly latestTransactionDateAcrossCountries = globalLastRunTime;

            foreach (var country in countries)
            {
                Console.WriteLine($"Starting detection for {country}. Last run time: {globalLastRunTime}");

                var dispositions = await detector.GetDispositionsAsync(dbContext, country);
                var (suspiciousUsers, latestTransactionDate) = detector.DetectSuspiciousActivity(dispositions, globalLastRunTime);

                var reportFilePath = Path.Combine(currentDirectory, "..", "..", "..", "SuspicionReport", $"report_{country}.txt");
                detector.GenerateReport(suspiciousUsers, reportFilePath, country);

                Console.WriteLine($"Detected {suspiciousUsers.Count} suspicious users for {country}.\n");
                Console.WriteLine("--------------------------------------------\n");
                latestTransactionDateAcrossCountries = latestTransactionDate > latestTransactionDateAcrossCountries ? latestTransactionDate : latestTransactionDateAcrossCountries;
            }

            detector.SaveLastRunTime(latestTransactionDateAcrossCountries, lastRunTimeFilePath);
            Console.WriteLine("Suspicious activity detection completed.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
}

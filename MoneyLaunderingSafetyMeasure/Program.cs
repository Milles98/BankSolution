using DataLibrary.Data;
using Microsoft.EntityFrameworkCore;
using MoneyLaunderingSafetyMeasure;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        try
        {
            var optionsBuilder = new DbContextOptionsBuilder<BankAppDataContext>();
            optionsBuilder.UseSqlServer("Server=localhost;Database=BankAppData;Trusted_Connection=True;TrustServerCertificate=true;MultipleActiveResultSets=true");

            using var dbContext = new BankAppDataContext(optionsBuilder.Options);
            var detector = new SuspiciousActivityDetector();

            var countries = new List<string> { "Sweden", "Norway", "Denmark", "Finland" };

            var currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var lastRunTimeFilePath = Path.Combine(currentDirectory, "..", "..", "..", "SuspicionReport", "lastRunTime.txt");

            foreach (var country in countries)
            {
                var lastRunTime = detector.GetLastRunTime(lastRunTimeFilePath);

                var dispositions = detector.GetDispositions(dbContext, country);
                var (suspiciousUsers, latestTransactionTime) = detector.DetectSuspiciousActivity(dispositions, lastRunTime);

                var reportFilePath = Path.Combine(currentDirectory, "..", "..", "..", "SuspicionReport", $"report_{country}.txt");
                detector.GenerateReport(suspiciousUsers, reportFilePath, country);

                detector.SaveLastRunTime(latestTransactionTime, lastRunTimeFilePath);
            }


            Console.WriteLine("Suspicious activity detection completed.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
}

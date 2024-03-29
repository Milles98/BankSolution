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
            var optionsBuilder = new DbContextOptionsBuilder<BankAppData2Context>();
            optionsBuilder.UseSqlServer("DefaultConnection");

            using var dbContext = new BankAppData2Context(optionsBuilder.Options);
            var detector = new SuspiciousActivityDetector();

            var countries = new List<string> { "Sweden", "Norway", "Denmark", "Finland" };

            var currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var lastRunTimeFilePath = Path.Combine(currentDirectory, "..", "..", "..", "SuspicionReport", "lastRunTime.txt");

            foreach (var country in countries)
            {
                var lastRunTime = detector.GetLastRunTime(lastRunTimeFilePath);

                var dispositions = detector.GetDispositions(dbContext, country);
                var suspiciousUsers = detector.DetectSuspiciousActivity(dispositions, lastRunTime);

                var reportFilePath = Path.Combine(currentDirectory, "..", "..", "..", "SuspicionReport", $"report_{country}.txt");
                detector.GenerateReport(suspiciousUsers, reportFilePath);

                detector.SaveLastRunTime(DateTime.Now, lastRunTimeFilePath);
            }

            Console.WriteLine("Suspicious activity detection completed.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
}

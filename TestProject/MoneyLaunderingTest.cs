using NUnit.Framework;
using System;
using DataLibrary.Data;
using MoneyLaunderingSafetyMeasure;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;

namespace TestProject
{
    public class MoneyLaunderingTest
    {
        private SuspiciousActivityDetector _detector;
        private BankAppDataContext _context;

        [SetUp]
        public void Setup()
        {
            var optionsBuilder = new DbContextOptionsBuilder<BankAppDataContext>();
            optionsBuilder.UseInMemoryDatabase("TestDb");
            _context = new BankAppDataContext(optionsBuilder.Options);
            _detector = new SuspiciousActivityDetector();
        }

        [Test]
        public void DetectSuspiciousActivity_WithSuspiciousTransactions_ShouldReturnSuspiciousUsers()
        {
            // Arrange
            var dispositions = new List<Disposition>
            {
                new Disposition
                {
                    Type = "Owner", // Set the Type property here
                    Customer = new Customer 
                    {
                        Givenname = "Test", 
                        Surname = "User", 
                        Country = "TestCountry",
                        City = "TestCity", // Set the City property here
                        CountryCode = "TC", // Set the CountryCode property here
                        Gender = "Male", // Set the Gender property here
                        Streetaddress = "TestStreet 123", // Set the Streetaddress property here
                        Zipcode = "12345" // Set the Zipcode property here
                    },
                    Account = new Account
                    {
                        Frequency = "Monthly", // Set the Frequency property here
                        Transactions = new List<Transaction>
                        {
                            new Transaction 
                            {
                                Amount = 16000,
                                Date = DateOnly.FromDateTime(DateTime.UtcNow),
                                Operation = "Credit", // Set the Operation property here
                                Type = "Debit" // Set the Type property here
                            },
                            new Transaction
                            {
                                Amount = 5000,
                                Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)),
                                Operation = "Credit", // Set the Operation property here
                                Type = "Debit" // Set the Type property here
                            }
                        }
                    }
                },
                // Add more dispositions as needed...
            };

            _context.Dispositions.AddRange(dispositions);
            _context.SaveChanges();

            // Act
            var (suspiciousUsers, _) = _detector.DetectSuspiciousActivity(_context.Dispositions.ToList(), DateOnly.MinValue);

            // Assert
            // Assert
            Assert.That(suspiciousUsers.Count, Is.GreaterThan(0));
            Assert.That(suspiciousUsers.Any(s => s.Contains("Test User") && s.Contains("'Single transaction exceeds limit'")));
        }

        [Test]
        public void GenerateReport_WithSuspiciousUsers_ShouldCreateReport()
        {
            // Arrange
            var suspiciousUsers = new List<string> { "Test User" };
            var reportFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "testReport.txt");

            // Act
            _detector.GenerateReport(suspiciousUsers, reportFilePath, "TestCountry");

            // Assert
            Assert.That(File.Exists(reportFilePath));
            Assert.That(File.ReadAllText(reportFilePath), Does.Contain("Test User"));
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }
    }
}
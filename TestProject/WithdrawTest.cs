using NUnit.Framework;
using System;
using DataLibrary.Data;
using DataLibrary.Services;
using Microsoft.EntityFrameworkCore;

namespace TestProject
{
    public class WithdrawTests
    {
        private BankService _bankService;
        private BankAppDataContext _context;

        [SetUp]
        public void Setup()
        {
            // Skapa en instans av DbContextOptionsBuilder
            var optionsBuilder = new DbContextOptionsBuilder<BankAppDataContext>();

            // Använd InMemory databas
            optionsBuilder.UseInMemoryDatabase("TestDb");

            // Skapa en instans av BankAppDataContext med de skapade inställningarna
            _context = new BankAppDataContext(optionsBuilder.Options);

            // Clear the Accounts in the InMemory database
            _context.Accounts.RemoveRange(_context.Accounts);

            // Lägg till data till din InMemory databas
            _context.Accounts.Add(new Account { AccountId = 1, Balance = 60000, Frequency = "Monthly" }); // Set the balance to 50000
            _context.SaveChanges();

            // Skapa en instans av BankService med den skapade kontexten
            _bankService = new BankService(_context);
        }

        [Test]
        public void Withdraw_WithValidAmount_ShouldDecreaseBalance()
        {
            // Arrange
            var accountId = 1; // Ändra detta till ett giltigt konto-ID i din databas
            var initialBalance = _context.Accounts.Find(accountId)!.Balance;
            var withdrawAmount = 100;

            // Act
            _bankService.Withdraw(accountId, withdrawAmount);

            // Assert
            var finalBalance = _context.Accounts.Find(accountId)!.Balance;
            Assert.That(finalBalance, Is.EqualTo(initialBalance - withdrawAmount));
        }

        [Test]
        public void Withdraw_WithInvalidAmount_ShouldThrowException()
        {
            // Arrange
            var accountId = 1; // Ändra detta till ett giltigt konto-ID i din databas
            decimal withdrawAmount = 0; // Mindre än det minsta tillåtna beloppet

            // Act & Assert
            var ex = Assert.Throws<Exception>(() => _bankService.Withdraw(accountId, withdrawAmount));
            Assert.That(ex.Message, Is.EqualTo("Withdraw amount must be greater than 0!"));
        }

        [Test]
        public void Withdraw_WithNonExistentAccount_ShouldThrowException()
        {
            // Arrange
            var nonExistentAccountId = 999; // An account ID that does not exist in the database
            var withdrawAmount = 100;

            // Act & Assert
            var ex = Assert.Throws<Exception>(() => _bankService.Withdraw(nonExistentAccountId, withdrawAmount));
            Assert.That(ex.Message, Is.EqualTo("Account not found"));
        }

        [Test]
        public void Withdraw_WithAmountGreaterThanMax_ShouldThrowException()
        {
            // Arrange
            var accountId = 1;
            decimal withdrawAmount = 50001; // Greater than the maximum allowed amount, but less than the balance of the account

            // Act & Assert
            var ex = Assert.Throws<Exception>(() => _bankService.Withdraw(accountId, withdrawAmount));
            Assert.That(ex.Message, Is.EqualTo("Withdraw amount must be less than or equal to 50,000 SEK at a time!"));
        }

        [Test]
        public void Withdraw_WithAmountGreaterThanBalance_ShouldThrowException()
        {
            // Arrange
            var accountId = 1;
            decimal withdrawAmount = 60001; // Greater than the balance of the account but less than the maximum allowed amount

            // Act & Assert
            var ex = Assert.Throws<Exception>(() => _bankService.Withdraw(accountId, withdrawAmount));
            Assert.That(ex.Message, Is.EqualTo("Insufficient balance"));
        }

        [Test]
        public void Withdraw_WithAmountEqualToMin_ShouldDecreaseBalance()
        {
            // Arrange
            var accountId = 1;
            decimal withdrawAmount = 1; // Equal to the minimum allowed amount
            var initialBalance = _context.Accounts.Find(accountId)!.Balance;

            // Act
            _bankService.Withdraw(accountId, withdrawAmount);

            // Assert
            var finalBalance = _context.Accounts.Find(accountId)!.Balance;
            Assert.That(finalBalance, Is.EqualTo(initialBalance - withdrawAmount));
        }

        [Test]
        public void Withdraw_WithAmountEqualToMax_ShouldDecreaseBalance()
        {
            // Arrange
            var accountId = 1;
            decimal withdrawAmount = 50000; // Equal to the maximum allowed amount
            var initialBalance = _context.Accounts.Find(accountId)!.Balance;

            // Act
            _bankService.Withdraw(accountId, withdrawAmount);

            // Assert
            var finalBalance = _context.Accounts.Find(accountId)!.Balance;
            Assert.That(finalBalance, Is.EqualTo(initialBalance - withdrawAmount));
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }
    }
}
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
            var optionsBuilder = new DbContextOptionsBuilder<BankAppDataContext>();

            optionsBuilder.UseInMemoryDatabase("TestDb");

            _context = new BankAppDataContext(optionsBuilder.Options);

            _context.Accounts.RemoveRange(_context.Accounts);

            _context.Accounts.Add(new Account { AccountId = 1, Balance = 60000, Frequency = "Monthly" });
            _context.SaveChanges();

            _bankService = new BankService(_context);
        }

        [Test]
        public void Withdraw_WithValidAmount_ShouldDecreaseBalance()
        {
            var accountId = 1;
            var initialBalance = _context.Accounts.Find(accountId)!.Balance;
            var withdrawAmount = 100;

            _bankService.Withdraw(accountId, withdrawAmount);

            var finalBalance = _context.Accounts.Find(accountId)!.Balance;
            Assert.That(finalBalance, Is.EqualTo(initialBalance - withdrawAmount));
        }

        [Test]
        public void Withdraw_WithInvalidAmount_ShouldThrowException()
        {
            var accountId = 1;
            decimal withdrawAmount = 0;

            var ex = Assert.Throws<Exception>(() => _bankService.Withdraw(accountId, withdrawAmount));
            Assert.That(ex.Message, Is.EqualTo("Withdraw amount must be greater than 0!"));
        }

        [Test]
        public void Withdraw_WithNonExistentAccount_ShouldThrowException()
        {
            var nonExistentAccountId = 999;
            var withdrawAmount = 100;

            var ex = Assert.Throws<Exception>(() => _bankService.Withdraw(nonExistentAccountId, withdrawAmount));
            Assert.That(ex.Message, Is.EqualTo("Account not found"));
        }

        [Test]
        public void Withdraw_WithAmountGreaterThanMax_ShouldThrowException()
        {
            var accountId = 1;
            decimal withdrawAmount = 50001;

            var ex = Assert.Throws<Exception>(() => _bankService.Withdraw(accountId, withdrawAmount));
            Assert.That(ex.Message, Is.EqualTo("Withdraw amount must be less than or equal to 50,000 SEK at a time!"));
        }

        [Test]
        public void Withdraw_WithAmountGreaterThanBalance_ShouldThrowException()
        {
            var accountId = 1;
            decimal withdrawAmount = 60001;

            var ex = Assert.Throws<Exception>(() => _bankService.Withdraw(accountId, withdrawAmount));
            Assert.That(ex.Message, Is.EqualTo("Insufficient balance"));
        }

        [Test]
        public void Withdraw_WithAmountEqualToMin_ShouldDecreaseBalance()
        {
            var accountId = 1;
            decimal withdrawAmount = 1;
            var initialBalance = _context.Accounts.Find(accountId)!.Balance;

            _bankService.Withdraw(accountId, withdrawAmount);

            var finalBalance = _context.Accounts.Find(accountId)!.Balance;
            Assert.That(finalBalance, Is.EqualTo(initialBalance - withdrawAmount));
        }

        [Test]
        public void Withdraw_WithAmountEqualToMax_ShouldDecreaseBalance()
        {
            var accountId = 1;
            decimal withdrawAmount = 50000;
            var initialBalance = _context.Accounts.Find(accountId)!.Balance;

            _bankService.Withdraw(accountId, withdrawAmount);

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
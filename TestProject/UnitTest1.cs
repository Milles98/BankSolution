using DataLibrary.Data;
using DataLibrary.Services;
using Microsoft.EntityFrameworkCore;

namespace TestProject;

[TestFixture]
public class BankServiceTests
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

        _context.Accounts.Add(new Account { AccountId = 1, Balance = 1000, Frequency = "Monthly" });
        _context.SaveChanges();

        _bankService = new BankService(_context);
    }

    [Test]
    public void Deposit_WithValidAmount_ShouldIncreaseBalance()
    {
        var accountId = 1;
        var initialBalance = _context.Accounts.Find(accountId)!.Balance;
        var depositAmount = 100;

        _bankService.Deposit(accountId, depositAmount);

        var finalBalance = _context.Accounts.Find(accountId)!.Balance;
        Assert.That(finalBalance, Is.EqualTo(initialBalance + depositAmount));
    }

    [Test]
    public void Deposit_WithInvalidAmount_ShouldThrowException()
    {
        var accountId = 1;
        decimal depositAmount = 40;

        var ex = Assert.Throws<Exception>(() => _bankService.Deposit(accountId, depositAmount));
        Assert.That(ex.Message, Is.EqualTo("Deposit amount cannot be less than 50 SEK"));
    }

    [Test]
    public void Deposit_WithNonExistentAccount_ShouldThrowException()
    {
        var nonExistentAccountId = 999;
        var depositAmount = 100;

        var ex = Assert.Throws<Exception>(() => _bankService.Deposit(nonExistentAccountId, depositAmount));
        Assert.That(ex.Message, Is.EqualTo("Account not found"));
    }

    [Test]
    public void Deposit_WithAmountGreaterThanMax_ShouldThrowException()
    {
        var accountId = 1;
        decimal depositAmount = 50001;

        var ex = Assert.Throws<Exception>(() => _bankService.Deposit(accountId, depositAmount));
        Assert.That(ex.Message, Is.EqualTo("Deposit amount must be less than or equal to 50,000 SEK at a time!"));
    }

    [Test]
    public void Deposit_WithAmountEqualToMin_ShouldIncreaseBalance()
    {
        var accountId = 1;
        decimal depositAmount = 50;
        var initialBalance = _context.Accounts.Find(accountId)!.Balance;

        _bankService.Deposit(accountId, depositAmount);

        var finalBalance = _context.Accounts.Find(accountId)!.Balance;
        Assert.That(finalBalance, Is.EqualTo(initialBalance + depositAmount));
    }

    [Test]
    public void Deposit_WithAmountEqualToMax_ShouldIncreaseBalance()
    {
        var accountId = 1;
        decimal depositAmount = 50000;
        var initialBalance = _context.Accounts.Find(accountId)!.Balance;

        _bankService.Deposit(accountId, depositAmount);

        var finalBalance = _context.Accounts.Find(accountId)!.Balance;
        Assert.That(finalBalance, Is.EqualTo(initialBalance + depositAmount));
    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }
}
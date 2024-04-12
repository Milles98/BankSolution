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
        // Skapa en instans av DbContextOptionsBuilder
        var optionsBuilder = new DbContextOptionsBuilder<BankAppDataContext>();

        // Använd InMemory databas
        optionsBuilder.UseInMemoryDatabase("TestDb");

        // Skapa en instans av BankAppDataContext med de skapade inställningarna
        _context = new BankAppDataContext(optionsBuilder.Options);

        // Clear the Accounts in the InMemory database
        _context.Accounts.RemoveRange(_context.Accounts);

        // Lägg till data till din InMemory databas
        _context.Accounts.Add(new Account { AccountId = 1, Balance = 1000, Frequency = "Monthly" });
        _context.SaveChanges();

        // Skapa en instans av BankService med den skapade kontexten
        _bankService = new BankService(_context);
    }

    [Test]
    public void Deposit_WithValidAmount_ShouldIncreaseBalance()
    {
        // Arrange
        var accountId = 1; // Ändra detta till ett giltigt konto-ID i din databas
        var initialBalance = _context.Accounts.Find(accountId)!.Balance;
        var depositAmount = 100;

        // Act
        _bankService.Deposit(accountId, depositAmount);

        // Assert
        var finalBalance = _context.Accounts.Find(accountId)!.Balance;
        Assert.That(finalBalance, Is.EqualTo(initialBalance + depositAmount));
    }

    [Test]
    public void Deposit_WithInvalidAmount_ShouldThrowException()
    {
        // Arrange
        var accountId = 1; // Ändra detta till ett giltigt konto-ID i din databas
        decimal depositAmount = 40; // Mindre än det minsta tillåtna beloppet

        // Act & Assert
        var ex = Assert.Throws<Exception>(() => _bankService.Deposit(accountId, depositAmount));
        Assert.That(ex.Message, Is.EqualTo("Deposit amount cannot be less than 50 SEK"));
    }
    
    [Test]
    public void Deposit_WithNonExistentAccount_ShouldThrowException()
    {
        // Arrange
        var nonExistentAccountId = 999; // An account ID that does not exist in the database
        var depositAmount = 100;

        // Act & Assert
        var ex = Assert.Throws<Exception>(() => _bankService.Deposit(nonExistentAccountId, depositAmount));
        Assert.That(ex.Message, Is.EqualTo("Account not found"));
    }

    [Test]
    public void Deposit_WithAmountGreaterThanMax_ShouldThrowException()
    {
        // Arrange
        var accountId = 1;
        decimal depositAmount = 50001; // Greater than the maximum allowed amount

        // Act & Assert
        var ex = Assert.Throws<Exception>(() => _bankService.Deposit(accountId, depositAmount));
        Assert.That(ex.Message, Is.EqualTo("Deposit amount must be less than or equal to 50,000 SEK at a time!"));
    }

    [Test]
    public void Deposit_WithAmountEqualToMin_ShouldIncreaseBalance()
    {
        // Arrange
        var accountId = 1;
        decimal depositAmount = 50; // Equal to the minimum allowed amount
        var initialBalance = _context.Accounts.Find(accountId)!.Balance;

        // Act
        _bankService.Deposit(accountId, depositAmount);

        // Assert
        var finalBalance = _context.Accounts.Find(accountId)!.Balance;
        Assert.That(finalBalance, Is.EqualTo(initialBalance + depositAmount));
    }

    [Test]
    public void Deposit_WithAmountEqualToMax_ShouldIncreaseBalance()
    {
        // Arrange
        var accountId = 1;
        decimal depositAmount = 50000; // Equal to the maximum allowed amount
        var initialBalance = _context.Accounts.Find(accountId)!.Balance;

        // Act
        _bankService.Deposit(accountId, depositAmount);

        // Assert
        var finalBalance = _context.Accounts.Find(accountId)!.Balance;
        Assert.That(finalBalance, Is.EqualTo(initialBalance + depositAmount));
    }
    
    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }
}
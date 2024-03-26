using DataLibrary.Data;
using DataLibrary.Services.Interfaces;

namespace DataLibrary.Services
{
    public class BankService : IBankService
    {
        private readonly BankAppData2Context _context;

        public BankService(BankAppData2Context context)
        {
            _context = context;
        }

        public int Deposit(int accountId, decimal amount)
        {
            if (amount < 0)
                throw new Exception("Deposit amount cannot be negative");

            var account = _context.Accounts.Find(accountId);
            if (account == null)
                throw new Exception("Account not found");

            account.Balance += amount;

            var transaction = new Transaction
            {
                AccountId = accountId,
                Amount = amount,
                Balance = account.Balance,
                Date = DateOnly.FromDateTime(DateTime.Today),
                Type = "Credit",
                Operation = "Deposit"
                // Set other transaction properties as needed
            };

            _context.Transactions.Add(transaction);
            _context.SaveChanges();

            return transaction.TransactionId;
        }

        public int Withdraw(int accountId, decimal amount)
        {
            if (amount < 0)
                throw new Exception("Withdrawal amount cannot be negative");

            var account = _context.Accounts.Find(accountId);
            if (account == null)
                throw new Exception("Account not found");

            if (account.Balance < amount)
                throw new Exception("Insufficient balance");

            account.Balance -= amount;

            var transaction = new Transaction
            {
                AccountId = accountId,
                Amount = -amount,
                Balance = account.Balance,
                Date = DateOnly.FromDateTime(DateTime.Today),
                Type = "Debit",
                Operation = "Withdraw"
                // Set other transaction properties as needed
            };

            _context.Transactions.Add(transaction);
            _context.SaveChanges();

            return transaction.TransactionId;
        }

        public void Transfer(int fromAccountId, int toAccountId, decimal amount)
        {
            if (amount < 0)
                throw new Exception("Transfer amount cannot be negative");

            Withdraw(fromAccountId, amount);
            Deposit(toAccountId, amount);
        }
    }
}

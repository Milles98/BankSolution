using DataLibrary.Data;
using DataLibrary.Services.Interfaces;
using DataLibrary.ViewModels;

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
            };

            _context.Transactions.Add(transaction);
            _context.SaveChanges();

            return transaction.TransactionId;
        }

        public int Withdraw(int accountId, decimal amount)
        {
            if (amount <= 0)
                throw new Exception("Withdraw amount must be greater than 0!");

            if (amount >= 50000)
                throw new Exception("Withdraw amount must be less than 50,000 SEK!");

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


        public int Transfer(int fromAccountId, int toAccountId, decimal amount)
        {
            if (amount < 0)
                throw new Exception("Transfer amount cannot be negative");

            Withdraw(fromAccountId, amount);
            return Deposit(toAccountId, amount);
        }

        public AccountViewModel GetAccountDetails(int accountId)
        {
            var account = _context.Accounts.FirstOrDefault(a => a.AccountId == accountId);
            if (account != null)
            {
                return new AccountViewModel
                {
                    AccountId = account.AccountId.ToString(),
                    Frequency = account.Frequency,
                    Created = account.Created.ToString(),
                    Balance = account.Balance,
                    Type = account.GetType().Name
                };
            }
            return null;
        }

        public AccountViewModel GetAccountDetailsForDisplay(int accountId)
        {
            if (accountId > 0)
            {
                var account = _context.Accounts.FirstOrDefault(a => a.AccountId == accountId);
                if (account != null)
                {
                    return new AccountViewModel
                    {
                        AccountId = account.AccountId.ToString(),
                        Frequency = account.Frequency,
                        Created = account.Created.ToString(),
                        Balance = account.Balance,
                        Type = account.GetType().Name
                    };
                }
            }
            return null;
        }

        public int DepositFunds(int accountId, decimal amount)
        {
            if (amount <= 0)
                throw new Exception("Deposit amount must be greater than 0!");

            if (amount >= 50000)
                throw new Exception("Deposit amount must be less than 50,000 SEK!");

            return Deposit(accountId, amount);
        }



    }
}

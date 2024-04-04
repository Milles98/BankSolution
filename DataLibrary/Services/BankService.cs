using DataLibrary.Data;
using DataLibrary.Services.Interfaces;
using DataLibrary.ViewModels;

namespace DataLibrary.Services
{
    public class BankService : IBankService
    {
        private readonly BankAppDataContext _context;

        public BankService(BankAppDataContext context)
        {
            _context = context;
        }

        public int Deposit(int accountId, decimal amount)
        {
            if (amount < 50)
                throw new Exception("Deposit amount cannot be less than 50 SEK");

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
                throw new Exception("Withdraw amount must be less than 50,000 SEK at a time!");

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
            };

            _context.Transactions.Add(transaction);
            _context.SaveChanges();

            return transaction.TransactionId;
        }


        public int Transfer(int fromAccountId, int toAccountId, decimal amount)
        {
            if (amount < 50)
                throw new Exception("Deposit amount cannot be less than 50 SEK");

            var fromAccount = _context.Accounts.Find(fromAccountId);
            if (fromAccount == null)
                throw new Exception("Source account not found");

            if (fromAccount.Balance < amount)
                throw new Exception("Insufficient balance");

            fromAccount.Balance -= amount;

            var withdrawTransaction = new Transaction
            {
                AccountId = fromAccountId,
                Amount = -amount,
                Balance = fromAccount.Balance,
                Date = DateOnly.FromDateTime(DateTime.Today),
                Type = "Debit",
                Operation = "Transfer"
            };

            var toAccount = _context.Accounts.Find(toAccountId);
            if (toAccount == null)
                throw new Exception("Target account not found");

            toAccount.Balance += amount;

            var depositTransaction = new Transaction
            {
                AccountId = toAccountId,
                Amount = amount,
                Balance = toAccount.Balance,
                Date = DateOnly.FromDateTime(DateTime.Today),
                Type = "Credit",
                Operation = "Transfer"
            };

            _context.Transactions.Add(withdrawTransaction);
            _context.Transactions.Add(depositTransaction);

            _context.SaveChanges();

            return depositTransaction.TransactionId;
        }

        public int TransferFunds(int fromAccountId, int toAccountId, decimal amount)
        {
            var fromAccount = _context.Accounts.First(a => a.AccountId == fromAccountId);
            if (fromAccount == null)
            {
                throw new Exception("From Account ID does not exist!");
            }

            var toAccount = _context.Accounts.First(a => a.AccountId == toAccountId);
            if (toAccount == null)
            {
                throw new Exception("To Account ID does not exist!");
            }

            if (fromAccountId == toAccountId)
            {
                throw new Exception("Cannot transfer to the same account!");
            }
            else if (amount <= 0)
            {
                throw new Exception("Amount must be greater than 0!");
            }
            else if (amount > fromAccount.Balance)
            {
                throw new Exception("Amount exceeds account balance!");
            }

            return Transfer(fromAccountId, toAccountId, amount);
        }



        public AccountViewModel GetAccountDetails(int accountId)
        {
            var account = _context.Accounts.First(a => a.AccountId == accountId);
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
                var account = _context.Accounts.First(a => a.AccountId == accountId);
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
            if (amount < 50)
                throw new Exception("Deposit amount cannot be less than 50 SEK");

            if (amount >= 50000)
                throw new Exception("Deposit amount must be less than 50,000 SEK at a time!s");

            return Deposit(accountId, amount);
        }



    }
}

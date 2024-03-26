namespace DataLibrary.Services.Interfaces
{
    public interface IBankService
    {
        int Deposit(int accountId, decimal amount);
        int Withdraw(int accountId, decimal amount);
        void Transfer(int fromAccountId, int toAccountId, decimal amount);
    }
}

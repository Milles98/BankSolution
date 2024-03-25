namespace BankWeb.ViewModel
{
    public class TransactionViewModel
    {
        public int AccountId { get; set; }
        public DateOnly DateOfTransaction { get; set; }
        public string Operation { get; set; }
        public decimal Amount { get; set; }
        public decimal Balance { get; set; }

    }
}

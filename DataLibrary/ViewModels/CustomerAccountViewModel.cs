namespace DataLibrary.ViewModels
{
    public class CustomerAccountViewModel
    {
        public int CustomerId { get; set; }
        public string Givenname { get; set; }
        public string Gender { get; set; } = null!;
        public string Surname { get; set; }
        public string Streetaddress { get; set; }
        public string City { get; set; }
        public List<AccountViewModel> Accounts { get; set; }
        public decimal TotalBalance { get; set; }
    }
}

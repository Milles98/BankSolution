using DataLibrary.Data;

namespace DataLibrary.ViewModels
{
    public class CustomerViewModel
    {
        public int CustomerId { get; set; }
        public int AccountId { get; set; }
        public string NationalId { get; set; } = null!;
        public string Givenname { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string Streetaddress { get; set; } = null!;
        public string City { get; set; } = null!;
        public string Country { get; set; } = null!;
        public List<Account> Accounts { get; set; }
    }
}

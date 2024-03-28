using DataLibrary.Data;

namespace DataLibrary.ViewModels
{
    public class CountryDetailsViewModel
    {
        public int CustomerId { get; set; }
        public string Givenname { get; set; }
        public string Surname { get; set; }
        public List<Disposition> Dispositions { get; set; }
        public decimal TotalBalance { get; set; }
    }
}


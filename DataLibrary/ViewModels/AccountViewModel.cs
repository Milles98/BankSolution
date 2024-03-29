using DataLibrary.Data;

namespace DataLibrary.ViewModels
{
    public class AccountViewModel
    {
        public string AccountId { get; set; }
        public string Frequency { get; set; }
        public string Created { get; set; }
        public decimal Balance { get; set; }
        public string Type { get; set; }
        public List<CustomerDispositionViewModel> Customers { get; set; }
    }
}

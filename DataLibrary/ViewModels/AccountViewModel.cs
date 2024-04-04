using DataLibrary.Data;
using System.ComponentModel.DataAnnotations;

namespace DataLibrary.ViewModels
{
    public class AccountViewModel
    {
        public string? AccountId { get; set; }
        public string? Frequency { get; set; }
        public string? Created { get; set; }
        [Range(50, 10000, ErrorMessage = "Initial deposit must be between 50 and 10.000 SEK.")]
        public decimal Balance { get; set; }
        public string? Type { get; set; }
        public List<CustomerDispositionViewModel>? Customers { get; set; }
        public int CustomerId { get; set; }
    }
}

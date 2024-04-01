using DataLibrary.Data;
using System.ComponentModel.DataAnnotations;

namespace DataLibrary.ViewModels
{
    public class AccountViewModel
    {
        public string? AccountId { get; set; }
        public string? Frequency { get; set; }
        public string? Created { get; set; }
        [Range(0, 5000, ErrorMessage = "Initial deposit must be between 0 and 5000.")]
        public decimal Balance { get; set; }
        public string? Type { get; set; }
        public List<CustomerDispositionViewModel>? Customers { get; set; }
        public int CustomerId { get; set; }
    }
}

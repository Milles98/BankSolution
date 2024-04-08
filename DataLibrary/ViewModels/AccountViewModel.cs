using DataLibrary.Data;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DataLibrary.ViewModels
{
    public class AccountViewModel
    {
        public string? AccountId { get; set; }

        [Required(ErrorMessage = "Frequency is required.")]
        public string? Frequency { get; set; }
        public string? Created { get; set; }

        [Range(50, 50000, ErrorMessage = "Initial deposit must be between 50 and 50.000 SEK.")]
        public decimal Balance { get; set; }
        public string? Type { get; set; }
        [JsonIgnore]
        public List<CustomerDispositionViewModel>? Customers { get; set; }
        public int CustomerId { get; set; }
    }
}

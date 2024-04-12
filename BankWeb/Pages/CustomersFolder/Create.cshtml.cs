using System.ComponentModel.DataAnnotations;
using DataLibrary.Data;
using DataLibrary.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BankWeb.Pages.CustomersFolder
{
    [Authorize(Roles = "Cashier")]
    public class CreateModel(IPersonService personService) : PageModel
    {
        [BindProperty]
        public Customer Customer { get; set; } = default!;
        [BindProperty]
        [Required]
        [RegularExpression("^(male|female)$", ErrorMessage = "Gender must be either 'male' or 'female'")]
        public string Gender { get; set; }

        [BindProperty]
        [Required]
        [StringLength(15, MinimumLength = 2, ErrorMessage = "Given name must be between 2 and 15 characters long")]
        public string Givenname { get; set; }
        [BindProperty]
        [Required]
        [StringLength(15, MinimumLength = 2, ErrorMessage = "Sur name must be between 2 and 15 characters long")]
        public string Surname { get; set; }
        [BindProperty]
        [Required]
        [StringLength(15, MinimumLength = 2, ErrorMessage = "Streetaddress must be between 2 and 15 characters long")]
        public string Streetaddress { get; set; }
        [BindProperty]
        [Required]
        [StringLength(15, MinimumLength = 2, ErrorMessage = "City must be between 2 and 15 characters long")]
        public string City { get; set; }
        [BindProperty]
        [Required]
        [RegularExpression(@"^(?=.*\d)([\d\s]*\d[\d\s]*){3,5}$", ErrorMessage = "Invalid Zipcode")]
        [StringLength(8, ErrorMessage = "Zipcode cannot be longer than 8 characters.")]
        public string Zipcode { get; set; }

        [BindProperty]
        [Required]
        [RegularExpression("^(Sweden|Norway|Denmark|Finland)$", ErrorMessage = "Country must be either 'Sweden', 'Norway', 'Denmark' or 'Finland'")]
        public string Country { get; set; }

        [BindProperty]
        [Required]
        [RegularExpression("^(OWNER|DISPONENT)$", ErrorMessage = "Disposition Type must be either 'OWNER' or 'DISPONENT'")]
        public string DispositionType { get; set; }
        [BindProperty]
        [Required]
        [RegularExpression("^(Monthly|Weekly|AfterTransaction)$", ErrorMessage = "Frequency must be either 'Monthly', 'Weekly' or 'After Transaction'")]
        public string Frequency { get; set; }
        [BindProperty]
        public int BirthdayYear { get; set; }

        [BindProperty]
        public int BirthdayMonth { get; set; }

        [BindProperty]
        public int BirthdayDay { get; set; }
        [BindProperty]
        [Required]
        [RegularExpression(@"^[\w-]+(\.[\w-]+)*@([\w-]+\.)+[a-zA-Z]{2,7}$", ErrorMessage = "Invalid email format")]
        public string Emailaddress { get; set; }
        [BindProperty]
        [Required]
        public string CountryCode { get; set; }
        [BindProperty]
        [Required]
        public string Telephonecountrycode { get; set; }

        [BindProperty]
        public string Telephonenumber { get; set; }

        [BindProperty]
        public string? NationalId { get; set; }
        [BindProperty]
        [Range(50, 50000, ErrorMessage = "Initial deposit must be between 50 and 50.000 SEK")]
        public decimal InitialDeposit { get; set; }

        public IActionResult OnGet()
        {
            return Page();
        }


        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var (customer, account, disposition) = await personService.CreateCustomerAsync(
                    Gender, Givenname, Surname, Streetaddress, City, Zipcode, Country, CountryCode, Emailaddress,
                    Telephonecountrycode, Telephonenumber, NationalId, BirthdayYear, BirthdayMonth, BirthdayDay,
                    Frequency, InitialDeposit, DispositionType);

                TempData["Message"] = $"Customer ID {customer.CustomerId} created along with Account ID {account.AccountId} and DispositionId {disposition.DispositionId} at {DateTime.Now:yyyy-MM-dd}";

                return RedirectToPage("/CustomersFolder/CustomerDetails", new { id = customer.CustomerId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return Page();
            }
        }

    }
}

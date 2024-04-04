using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using DataLibrary.Data;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using DataLibrary.Services.Interfaces;

namespace BankWeb.Pages.CustomerCRUD
{
    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly IPersonService _personService;

        public CreateModel(IPersonService personService)
        {
            _personService = personService;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

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
        [StringLength(15, MinimumLength = 2, ErrorMessage = "Streetaddress must be between 2 and 30 characters long")]
        public string Streetaddress { get; set; }
        [BindProperty]
        [Required]
        [StringLength(15, MinimumLength = 2, ErrorMessage = "City must be between 2 and 15 characters long")]
        public string City { get; set; }
        [BindProperty]
        [Required]
        [RegularExpression(@"^\d{1,5}(?:\s*\d{1,5})*$", ErrorMessage = "Zipcode must be exactly 5 digits long")]
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

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var existingCustomer = await _personService.GetCustomerByEmailAsync(Emailaddress);
            if (existingCustomer != null)
            {
                ModelState.AddModelError("Emailaddress", "A customer with this email address already exists.");
                return Page();
            }

            var customer = new Customer
            {
                Gender = Gender,
                Givenname = Givenname,
                Surname = Surname,
                Streetaddress = Streetaddress,
                City = City,
                Zipcode = Zipcode,
                Country = Country,
                CountryCode = CountryCode,
                Emailaddress = Emailaddress,
                Telephonecountrycode = Telephonecountrycode,
                Telephonenumber = Telephonenumber,
                NationalId = NationalId
            };

            if (BirthdayYear > 0 && BirthdayMonth > 0 && BirthdayDay > 0)
            {
                customer.Birthday = new DateOnly(BirthdayYear, BirthdayMonth, BirthdayDay);
            }

            var account = new Account
            {
                Frequency = Frequency,
                Created = DateOnly.FromDateTime(DateTime.Today),
                Balance = 0,
            };

            var disposition = new Disposition
            {
                Customer = customer,
                Account = account,
                Type = DispositionType
            };

            customer = await _personService.CreateCustomerAsync(customer, account, disposition);

            TempData["Message"] = $"Customer ID {customer.CustomerId} created along with Account ID {account.AccountId} and DispositionId {disposition.DispositionId} at {DateTime.Now:yyyy-MM-dd}";

            return RedirectToPage("/CustomersFolder/CustomerDetails", new { id = customer.CustomerId });
        }

    }
}

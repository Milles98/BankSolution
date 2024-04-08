using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DataLibrary.Data;

namespace BankWeb.Pages.CustomerCRUD
{
    [BindProperties]
    public class EditModel : PageModel
    {
        private readonly BankAppDataContext _context;

        public EditModel(BankAppDataContext context)
        {
            _context = context;
        }

        [Required] public string Gender { get; set; }
        [StringLength(15, MinimumLength = 2)] public string Givenname { get; set; }
        [StringLength(15, MinimumLength = 2)] public string Surname { get; set; }
        [StringLength(30, MinimumLength = 2)] public string Streetaddress { get; set; }
        [StringLength(15, MinimumLength = 2)] public string City { get; set; }
        [RegularExpression(@"^(?=.*\d)([\d\s]*\d[\d\s]*){3,5}$", ErrorMessage = "Invalid Zipcode")]
        [StringLength(8, ErrorMessage = "Zipcode cannot be longer than 8 characters.")]
        public string Zipcode { get; set; }

        [RegularExpression("^(Sweden|Norway|Denmark|Finland)$")] public string Country { get; set; }
        public string CountryCode { get; set; }
        public int BirthdayYear { get; set; }
        public int BirthdayMonth { get; set; }
        public int BirthdayDay { get; set; }
        public string? NationalId { get; set; }
        public string Telephonecountrycode { get; set; }
        public string Telephonenumber { get; set; }
        [RegularExpression(@"^[\w-]+(\.[\w-]+)*@([\w-]+\.)+[a-zA-Z]{2,7}$")] public string Emailaddress { get; set; }

        public IActionResult OnGet(int id)
        {
            var customerDb = _context.Customers.FirstOrDefault(m => m.CustomerId == id);

            if (customerDb == null)
            {
                return NotFound($"No customer found with ID {id}.");
            }

            Gender = customerDb.Gender;
            Givenname = customerDb.Givenname;
            Surname = customerDb.Surname;
            Streetaddress = customerDb.Streetaddress;
            City = customerDb.City;
            Zipcode = customerDb.Zipcode;
            Country = customerDb.Country;
            CountryCode = customerDb.CountryCode;
            Telephonecountrycode = customerDb.Telephonecountrycode;
            Telephonenumber = customerDb.Telephonenumber;
            Emailaddress = customerDb.Emailaddress;
            NationalId = customerDb.NationalId;

            if (customerDb.Birthday.HasValue)
            {
                BirthdayYear = customerDb.Birthday.Value.Year;
                BirthdayMonth = customerDb.Birthday.Value.Month;
                BirthdayDay = customerDb.Birthday.Value.Day;
            }

            return Page();
        }


        public IActionResult OnPost(int id)
        {
            if (ModelState.IsValid)
            {
                var customerDb = _context.Customers.First(m => m.CustomerId == id);

                var oldValues = new
                {
                    customerDb.Gender,
                    customerDb.Givenname,
                    customerDb.Surname,
                    customerDb.Streetaddress,
                    customerDb.City,
                    customerDb.Zipcode,
                    customerDb.Country,
                    customerDb.CountryCode,
                    customerDb.Telephonecountrycode,
                    customerDb.Telephonenumber,
                    customerDb.Emailaddress,
                    customerDb.NationalId,
                    customerDb.Birthday
                };

                customerDb.Gender = Gender;
                customerDb.Givenname = Givenname;
                customerDb.Surname = Surname;
                customerDb.Streetaddress = Streetaddress;
                customerDb.City = City;
                customerDb.Zipcode = Zipcode;
                customerDb.Country = Country;
                customerDb.CountryCode = CountryCode;
                customerDb.Telephonecountrycode = Telephonecountrycode;
                customerDb.Telephonenumber = Telephonenumber;
                customerDb.Emailaddress = Emailaddress;
                customerDb.NationalId = NationalId;

                if (BirthdayYear > 0 && BirthdayMonth > 0 && BirthdayDay > 0)
                {
                    customerDb.Birthday = new DateOnly(BirthdayYear, BirthdayMonth, BirthdayDay);
                }

                var changes = new List<string>();
                if (oldValues.Gender != Gender) changes.Add($"Gender: {oldValues.Gender} -> {Gender}");
                if (oldValues.Givenname != Givenname) changes.Add($"Givenname: {oldValues.Givenname} -> {Givenname}");
                if (oldValues.Surname != Surname) changes.Add($"Surname: {oldValues.Surname} -> {Surname}");
                if (oldValues.Streetaddress != Streetaddress) changes.Add($"Streetaddress: {oldValues.Streetaddress} -> {Streetaddress}");
                if (oldValues.City != City) changes.Add($"City: {oldValues.City} -> {City}");
                if (oldValues.Zipcode != Zipcode) changes.Add($"Zipcode: {oldValues.Zipcode} -> {Zipcode}");
                if (oldValues.Country != Country) changes.Add($"Country: {oldValues.Country} -> {Country}");
                if (oldValues.CountryCode != CountryCode) changes.Add($"CountryCode: {oldValues.CountryCode} -> {CountryCode}");
                if (oldValues.Telephonecountrycode != Telephonecountrycode) changes.Add($"Telephonecountrycode: {oldValues.Telephonecountrycode} -> {Telephonecountrycode}");
                if (oldValues.Telephonenumber != Telephonenumber) changes.Add($"Telephonenumber: {oldValues.Telephonenumber} -> {Telephonenumber}");
                if (oldValues.Emailaddress != Emailaddress) changes.Add($"Emailaddress: {oldValues.Emailaddress} -> {Emailaddress}");
                if (oldValues.NationalId != NationalId) changes.Add($"NationalId: {oldValues.NationalId} -> {NationalId}");
                if (oldValues.Birthday != customerDb.Birthday) changes.Add($"Birthday: {oldValues.Birthday} -> {customerDb.Birthday}");

                if (changes.Count > 0)
                {
                    _context.Attach(customerDb).State = EntityState.Modified;
                    _context.SaveChanges();

                    TempData["Message"] = $"Customer ID {customerDb.CustomerId} ({customerDb.Givenname} {customerDb.Surname}) successfully updated. Changes: {string.Join(", ", changes)}";
                }
                else
                {
                    TempData["ErrorMessage"] = "Nothing has been changed.";
                }

                return RedirectToPage("/CustomersFolder/CustomerDetails", new { id = customerDb.CustomerId });
            }
            return Page();
        }
    }
}

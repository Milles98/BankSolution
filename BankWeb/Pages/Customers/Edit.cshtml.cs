﻿using System.ComponentModel.DataAnnotations;
using DataLibrary.Validation;
using DataLibrary.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BankWeb.Pages.Customers
{
    [BindProperties]
    public class EditModel(IPersonService personService) : PageModel
    {
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
        [MinimumAge(18)]
        public int BirthdayYear { get; set; }
        public int BirthdayMonth { get; set; }
        public int BirthdayDay { get; set; }
        public string? NationalId { get; set; }
        public string Telephonecountrycode { get; set; }
        public string Telephonenumber { get; set; }
        [RegularExpression(@"^[\w-]+(\.[\w-]+)*@([\w-]+\.)+[a-zA-Z]{2,7}$")] public string Emailaddress { get; set; }

        public IActionResult OnGet(int id)
        {
            var customerFromDb = personService.GetDbContext().Customers.First(m => m.CustomerId == id);

            if (customerFromDb == null)
            {
                return NotFound($"No customer found with ID {id}.");
            }

            Gender = customerFromDb.Gender;
            Givenname = customerFromDb.Givenname;
            Surname = customerFromDb.Surname;
            Streetaddress = customerFromDb.Streetaddress;
            City = customerFromDb.City;
            Zipcode = customerFromDb.Zipcode;
            Country = customerFromDb.Country;
            CountryCode = customerFromDb.CountryCode;
            Telephonecountrycode = customerFromDb.Telephonecountrycode;
            Telephonenumber = customerFromDb.Telephonenumber;
            Emailaddress = customerFromDb.Emailaddress;
            NationalId = customerFromDb.NationalId;

            if (customerFromDb.Birthday.HasValue)
            {
                BirthdayYear = customerFromDb.Birthday.Value.Year;
                BirthdayMonth = customerFromDb.Birthday.Value.Month;
                BirthdayDay = customerFromDb.Birthday.Value.Day;
            }

            ViewData["CustomerId"] = id;

            return Page();
        }


        public async Task<IActionResult> OnPostAsync(int id)
        {
            ViewData["CustomerId"] = id;

            if (ModelState.IsValid)
            {
                try
                {
                    var (customer, changes) = await personService.UpdateCustomerAsync(
                        id, Gender, Givenname, Surname, Streetaddress, City, Zipcode, Country, CountryCode,
                        Emailaddress, Telephonecountrycode, Telephonenumber, NationalId, BirthdayYear, BirthdayMonth, BirthdayDay
                    );

                    if (changes.Count > 0)
                    {
                        TempData["EditMessage"] = $"Customer ID {customer.CustomerId} ({customer.Givenname} {customer.Surname}) successfully updated.<br>{string.Join("<br>", changes)}";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "You saved but didnt update customer, nothing has been changed.";
                    }

                    return RedirectToPage("/Customers/CustomerDetails", new { id = customer.CustomerId });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                    return Page();
                }
            }

            return Page();
        }
    }
}
using DataLibrary.Data;
using DataLibrary.Services.Interfaces;
using DataLibrary.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BankWeb.Pages
{
    [ResponseCache(Duration = 60, VaryByQueryKeys = new[] { "country" })]
    public class CountryDetailsModel : PageModel
    {
        private readonly ICountryService _countryService;

        public CountryDetailsModel(ICountryService countryService)
        {
            _countryService = countryService;
        }

        public string Country { get; set; }
        public string ImagePath { get; set; }

        public List<CountryDetailsViewModel> TopCustomers { get; set; }

        public async Task OnGet(string country)
        {
            Country = country;
            switch (country)
            {
                case "Denmark":
                    ImagePath = "/assets/images/denmark.jpg";
                    break;
                case "Norway":
                    ImagePath = "/assets/images/norway.png";
                    break;
                case "Sweden":
                    ImagePath = "/assets/images/sweden.svg";
                    break;
                case "Finland":
                    ImagePath = "/assets/images/finland.jpg";
                    break;
                default:
                    ImagePath = "/assets/images/default.jpg";
                    break;
            }
            TopCustomers = await _countryService.GetTopCustomersByCountry(country);
        }
    }

}

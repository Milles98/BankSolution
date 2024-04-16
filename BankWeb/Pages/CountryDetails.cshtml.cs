using DataLibrary.Data;
using DataLibrary.Services.Interfaces;
using DataLibrary.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;


namespace BankWeb.Pages
{
    [IgnoreAntiforgeryToken]
    [ResponseCache(Duration = 60, VaryByQueryKeys = new[] { "country" })]
    public class CountryDetailsModel : PageModel
    {
        private readonly ILogger<CountryDetailsModel> _logger;
        private readonly ICountryService _countryService;

        public CountryDetailsModel(ICountryService countryService, ILogger<CountryDetailsModel> logger)
        {
            _countryService = countryService;
            _logger = logger;
        }
        public string Country { get; set; }
        public string ImagePath { get; set; }

        public List<CountryDetailsViewModel> TopCustomers { get; set; }

        public async Task OnGet(string country)
        {
            _logger.LogInformation("OnGet called with country: {Country}", country);

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
            _logger.LogInformation("TopCustomers count: {Count}", TopCustomers.Count);
        }
    }

}

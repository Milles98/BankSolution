using BankWeb.ViewModels;
using DataLibrary.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BankWeb.Pages
{
    [ResponseCache(Duration = 60, VaryByQueryKeys = new[] { "country" })]
    public class CountryDetailsModel : PageModel
    {
        private readonly BankAppData2Context _context;

        public CountryDetailsModel(BankAppData2Context context)
        {
            _context = context;
        }

        public string Country { get; set; }
        public string ImagePath { get; set; }

        public List<CountryDetailsViewModel> TopCustomers { get; set; }

        public void OnGet(string country)
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
            TopCustomers = _context.Customers
                .Include(c => c.Dispositions)
                .ThenInclude(d => d.Account)
                .Where(c => c.Country == country)
                .OrderByDescending(c => c.Dispositions.Sum(d => d.Account.Balance))
                .Select(c => new CountryDetailsViewModel
                {
                    CustomerId = c.CustomerId,
                    Givenname = c.Givenname,
                    Surname = c.Surname,
                    Dispositions = c.Dispositions.ToList(),
                    TotalBalance = c.Dispositions.Sum(d => d.Account.Balance)
                })
                .Take(10)
                .ToList();
        }
    }

}

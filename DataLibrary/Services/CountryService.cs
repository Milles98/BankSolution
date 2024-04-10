using DataLibrary.Data;
using DataLibrary.Services.Interfaces;
using DataLibrary.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLibrary.Services
{
    public class CountryService : ICountryService
    {
        private readonly BankAppDataContext _context;

        public CountryService(BankAppDataContext context)
        {
            _context = context;
        }

        public async Task<List<CountryDetailsViewModel>> GetTopCustomersByCountry(string country)
        {

            return await _context.Customers
                .Include(c => c.Dispositions)
                .ThenInclude(d => d.Account)
                .Where(c => c.Country == country)
                .OrderByDescending(c => c.Dispositions.Sum(d => d.Account.Balance))
                .Select(c => new CountryDetailsViewModel
                {
                    CustomerId = c.CustomerId,
                    Givenname = c.Givenname,
                    Surname = c.Surname,
                    TotalBalance = c.Dispositions.Sum(d => d.Account.Balance)
                })
                .Take(10)
                .ToListAsync();
        }
    }
}

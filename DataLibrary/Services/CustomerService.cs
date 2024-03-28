using DataLibrary.Data;
using DataLibrary.Services.Interfaces;
using DataLibrary.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace DataLibrary.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly BankAppData2Context _context;

        public CustomerService(BankAppData2Context context)
        {
            _context = context;
        }
        public async Task<CustomerAccountViewModel> GetCustomerDetails(int id)
        {
            var customer = await _context.Customers
                .Include(c => c.Dispositions)
                .ThenInclude(d => d.Account)
                .FirstOrDefaultAsync(m => m.CustomerId == id);

            if (customer == null)
            {
                return null;
            }

            var customerDetails = new CustomerAccountViewModel
            {
                CustomerId = customer.CustomerId,
                Givenname = customer.Givenname,
                Surname = customer.Surname,
                Streetaddress = customer.Streetaddress,
                City = customer.City,
                Accounts = customer.Dispositions.Select(d => new AccountViewModel
                {
                    AccountId = d.Account.AccountId.ToString(),
                    Frequency = d.Account.Frequency,
                    Created = d.Account.Created.ToString("yyyy-MM-dd"),
                    Balance = d.Account.Balance,
                    Type = d.Type
                }).ToList(),
                TotalBalance = customer.Dispositions.Sum(d => d.Account.Balance)
            };

            return customerDetails;
        }

    }
}

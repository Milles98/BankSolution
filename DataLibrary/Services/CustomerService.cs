using DataLibrary.Data;
using DataLibrary.Services.Interfaces;
using DataLibrary.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace DataLibrary.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly BankAppData2Context _context;
        private readonly IPaginationService<Customer> _paginationService;

        public CustomerService(BankAppData2Context context, IPaginationService<Customer> paginationService)
        {
            _context = context;
            _paginationService = paginationService;
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

        public async Task<List<CustomerViewModel>> GetCustomers(int currentPage, int customersPerPage, string search)
        {
            var query = _context.Customers.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                if (int.TryParse(search, out int customerId))
                {
                    query = query.Where(c => c.CustomerId == customerId);
                }
            }

            var customers = await _paginationService
                .GetPage(query
                .Include(c => c.Dispositions)
                .ThenInclude(d => d.Account), currentPage, customersPerPage)
                .Select(c => new CustomerViewModel
                {
                    CustomerId = c.CustomerId,
                    AccountId = c.Dispositions.Select(d => d.AccountId).FirstOrDefault(),
                    Givenname = c.Givenname,
                    Surname = c.Surname,
                    Streetaddress = c.Streetaddress,
                    City = c.City,
                    Accounts = c.Dispositions.Select(d => d.Account).ToList()
                }).ToListAsync();

            return customers;
        }

        public int GetTotalPages(int customersPerPage)
        {
            var query = _context.Customers.AsQueryable();
            return _paginationService.GetTotalPages(query, customersPerPage);
        }



    }
}

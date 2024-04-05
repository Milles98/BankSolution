using DataLibrary.Data;
using DataLibrary.Services.Interfaces;
using DataLibrary.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DataLibrary.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly BankAppDataContext _context;
        private readonly IPaginationService<Customer> _paginationService;
        private readonly ISortingService<Customer> _sortingService;

        public CustomerService(BankAppDataContext context, IPaginationService<Customer> paginationService, ISortingService<Customer> sortingService)
        {
            _context = context;
            _paginationService = paginationService;
            _sortingService = sortingService;
        }

        public int GetTotalCustomers()
        {
            return _context.Customers.Count();
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
                Gender = customer.Gender,
                Streetaddress = customer.Streetaddress,
                City = customer.City,
                Country = customer.Country,
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

        public async Task<(List<CustomerViewModel>, int)> GetCustomers(int currentPage, int customersPerPage, string sortColumn, string sortOrder, string search)
        {
            var query = _context.Customers.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                if (int.TryParse(search, out int customerId))
                {
                    query = query.Where(c => c.CustomerId == customerId);
                }
                else
                {
                    query = query.Where(c => c.Givenname.ToLower().Contains(search) ||
                                             c.Surname.ToLower().Contains(search) ||
                                             c.City.ToLower().Contains(search));
                }
            }

            int totalCustomers = await query.CountAsync();

            var sortExpressions = new Dictionary<string, Expression<Func<Customer, object>>>
            {
                { "CustomerId", c => c.CustomerId },
                { "AccountId", c => c.Dispositions.Select(d => d.AccountId).First() },
                { "NationalId", c => c.NationalId },
                { "Givenname", c => c.Givenname },
                { "Surname", c => c.Surname },
                { "Streetaddress", c => c.Streetaddress },
                { "City", c => c.City },

            };

            query = _sortingService.Sort(query, sortColumn, sortOrder, sortExpressions);

            var customers = await _paginationService
                .GetPage(query
                .Include(c => c.Dispositions)
                .ThenInclude(d => d.Account), currentPage, customersPerPage)
                .Select(c => new CustomerViewModel
                {
                    CustomerId = c.CustomerId,
                    AccountId = c.Dispositions.Select(d => d.AccountId).First(),
                    NationalId = c.NationalId,
                    Givenname = c.Givenname,
                    Surname = c.Surname,
                    Streetaddress = c.Streetaddress,
                    City = c.City,
                    Accounts = c.Dispositions.Select(d => d.Account).ToList()
                }).ToListAsync();

            return (customers, totalCustomers);
        }


        public int GetTotalPages(int customersPerPage)
        {
            var query = _context.Customers.AsQueryable();
            return _paginationService.GetTotalPages(query, customersPerPage);
        }



    }
}

using AutoMapper;
using DataLibrary.Data;
using DataLibrary.Infrastructure.Paging;
using DataLibrary.Services.Interfaces;
using DataLibrary.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DataLibrary.Services
{
    public class CustomerService(
        BankAppDataContext context,
        ISortingService<Customer> sortingService,
        IMapper mapper)
        : ICustomerService
    {
        public int GetTotalCustomers()
        {
            return context.Customers.Count();
        }
        public async Task<CustomerAccountViewModel> GetCustomerDetails(int id)
        {
            var customer = await context.Customers
                .Include(c => c.Dispositions)
                .ThenInclude(d => d.Account)
                .FirstOrDefaultAsync(m => m.CustomerId == id);

            if (customer == null)
            {
                return null;
            }

            var customerDetails = mapper.Map<CustomerAccountViewModel>(customer);

            return customerDetails;
        }

        public async Task<PagedResult<CustomerViewModel>> GetCustomers(int pageNo, int customersPerPage, string sortColumn, string sortOrder, string search)
        {
            var query = context.Customers.AsQueryable();

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

            query = sortingService.Sort(query, sortColumn, sortOrder, sortExpressions);

            var customers = await query
                .Include(c => c.Dispositions)
                .ThenInclude(d => d.Account)
                .GetPaged(pageNo, 50);

            var customerViewModels = mapper.Map<List<CustomerViewModel>>(customers.Results);

            return new PagedResult<CustomerViewModel>
            {
                CurrentPage = customers.CurrentPage,
                PageCount = customers.PageCount,
                PageSize = customers.PageSize,
                RowCount = customers.RowCount,
                Results = customerViewModels
            };
        }



    }
}

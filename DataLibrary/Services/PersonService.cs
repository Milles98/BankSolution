using DataLibrary.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLibrary.Data;
using Microsoft.EntityFrameworkCore;

namespace DataLibrary.Services
{
    public class PersonService : IPersonService
    {
        private readonly BankAppDataContext _context;

        public PersonService(BankAppDataContext context)
        {
            _context = context;
        }

        public async Task<Customer> CreateCustomerAsync(Customer customer, Account account, Disposition disposition)
        {
            account.Dispositions.Add(disposition);
            _context.Accounts.Add(account);
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return customer;
        }

        public async Task DeleteCustomerAsync(int id)
        {
            var customer = await _context.Customers.Include(c => c.Dispositions)
                                                   .ThenInclude(d => d.Account)
                                                   .FirstAsync(c => c.CustomerId == id);
            if (customer != null)
            {
                // Remove all related dispositions and their accounts
                foreach (var disposition in customer.Dispositions.ToList())
                {
                    _context.Dispositions.Remove(disposition);
                    _context.Accounts.Remove(disposition.Account);
                }

                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Customer> GetCustomerAsync(int id)
        {
            return await _context.Customers.FirstAsync(m => m.CustomerId == id);
        }
    }
}

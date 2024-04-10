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
                                                   .ThenInclude(a => a.Transactions)
                                                   .FirstAsync(c => c.CustomerId == id);
            if (customer != null)
            {
                foreach (var disposition in customer.Dispositions.ToList())
                {
                    _context.Transactions.RemoveRange(disposition.Account.Transactions);
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

        public async Task<Customer?> GetCustomerByEmailAsync(string email)
        {
            return await _context.Customers.FirstOrDefaultAsync(c => c.Emailaddress == email);
        }

        public async Task<bool> IsUniqueCombination(string givenName, string surname, string streetAddress)
        {
            return !await _context.Customers.AnyAsync(c => c.Givenname == givenName &&
                                                          c.Surname == surname &&
                                                          c.Streetaddress == streetAddress);
        }

        public async Task<(Customer, Account, Disposition)> CreateCustomerAsync(
            string gender, string givenName, string surname, string streetAddress, string city, string zipcode,
            string country, string countryCode, string emailaddress, string telephoneCountryCode, string telephoneNumber,
            string? nationalId, int birthdayYear, int birthdayMonth, int birthdayDay, string frequency, decimal initialDeposit,
            string dispositionType)
        {
            var existingCustomer = await GetCustomerByEmailAsync(emailaddress);
            if (existingCustomer != null)
            {
                throw new Exception("A customer with this email address already exists.");
            }

            var isUnique = await IsUniqueCombination(givenName, surname, streetAddress);
            if (!isUnique)
            {
                throw new Exception("A customer with this name and address combination already exists.");
            }

            var customer = new Customer
            {
                Gender = gender,
                Givenname = givenName,
                Surname = surname,
                Streetaddress = streetAddress,
                City = city,
                Zipcode = zipcode,
                Country = country,
                CountryCode = countryCode,
                Emailaddress = emailaddress,
                Telephonecountrycode = telephoneCountryCode,
                Telephonenumber = telephoneNumber,
                NationalId = nationalId
            };

            if (birthdayYear > 0 && birthdayMonth > 0 && birthdayDay > 0)
            {
                try
                {
                    customer.Birthday = new DateOnly(birthdayYear, birthdayMonth, birthdayDay);
                }
                catch (ArgumentOutOfRangeException)
                {
                    throw new Exception("Invalid date of birth");
                }
            }

            var account = new Account
            {
                Frequency = frequency,
                Created = DateOnly.FromDateTime(DateTime.Today),
                Balance = initialDeposit
            };

            var disposition = new Disposition
            {
                Customer = customer,
                Account = account,
                Type = dispositionType
            };

            var completeCustomer = await CreateCustomerAsync(customer, account, disposition);
            return (completeCustomer, account, disposition);
        }

    }
}

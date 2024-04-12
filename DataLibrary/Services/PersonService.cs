using DataLibrary.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLibrary.Data;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;
using System.Net.Mail;
using System.Reflection.Emit;
using System.Reflection;

namespace DataLibrary.Services
{
    public class PersonService(BankAppDataContext context) : IPersonService
    {
        public BankAppDataContext GetDbContext()
        {
            return context;
        }

        public async Task<Customer> CreateCustomerAsync(Customer customer, Account account, Disposition disposition)
        {
            account.Dispositions.Add(disposition);
            context.Accounts.Add(account);
            context.Customers.Add(customer);
            await context.SaveChangesAsync();

            return customer;
        }

        public async Task DeleteCustomerAsync(int id)
        {
            var customer = await context.Customers.Include(c => c.Dispositions)
                                                   .ThenInclude(d => d.Account)
                                                   .ThenInclude(a => a.Transactions)
                                                   .FirstAsync(c => c.CustomerId == id);
            if (customer != null)
            {
                foreach (var disposition in customer.Dispositions.ToList())
                {
                    context.Transactions.RemoveRange(disposition.Account.Transactions);
                    context.Dispositions.Remove(disposition);
                    context.Accounts.Remove(disposition.Account);
                }

                context.Customers.Remove(customer);
                await context.SaveChangesAsync();
            }
        }

        public async Task<Customer> GetCustomerAsync(int id)
        {
            return await context.Customers.FirstAsync(m => m.CustomerId == id);
        }

        public async Task<Customer?> GetCustomerByEmailAsync(string email)
        {
            return await context.Customers.FirstOrDefaultAsync(c => c.Emailaddress == email);
        }

        public async Task<bool> IsUniqueCombination(string givenName, string surname, string streetAddress)
        {
            return !await context.Customers.AnyAsync(c => c.Givenname == givenName &&
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

        public async Task<(Customer, List<string>)> UpdateCustomerAsync(
            int id, string gender, string givenName, string surname, string streetAddress, string city, string zipcode,
            string country, string countryCode, string emailaddress, string telephoneCountryCode, string telephoneNumber,
            string? nationalId, int birthdayYear, int birthdayMonth, int birthdayDay
        )
        {
            var customerFromDb = await context.Customers.FirstAsync(m => m.CustomerId == id);

            var oldValues = new
            {
                customerFromDb.Gender,
                customerFromDb.Givenname,
                customerFromDb.Surname,
                customerFromDb.Streetaddress,
                customerFromDb.City,
                customerFromDb.Zipcode,
                customerFromDb.Country,
                customerFromDb.CountryCode,
                customerFromDb.Telephonecountrycode,
                customerFromDb.Telephonenumber,
                customerFromDb.Emailaddress,
                customerFromDb.NationalId,
                customerFromDb.Birthday
            };

            customerFromDb.Gender = gender;
            customerFromDb.Givenname = givenName;
            customerFromDb.Surname = surname;
            customerFromDb.Streetaddress = streetAddress;
            customerFromDb.City = city;
            customerFromDb.Zipcode = zipcode;
            customerFromDb.Country = country;
            customerFromDb.CountryCode = countryCode;
            customerFromDb.Telephonecountrycode = telephoneCountryCode;
            customerFromDb.Telephonenumber = telephoneNumber;
            customerFromDb.Emailaddress = emailaddress;
            customerFromDb.NationalId = nationalId;

            if (birthdayYear > 0 && birthdayMonth > 0 && birthdayDay > 0)
            {
                customerFromDb.Birthday = new DateOnly(birthdayYear, birthdayMonth, birthdayDay);
            }

            var changes = new List<string>();
            if (oldValues.Gender != gender) changes.Add($"Gender: {oldValues.Gender} -> {gender}");
            if (oldValues.Givenname != givenName) changes.Add($"Givenname: {oldValues.Givenname} -> {givenName}");
            if (oldValues.Surname != surname) changes.Add($"Surname: {oldValues.Surname} -> {surname}");
            if (oldValues.Streetaddress != streetAddress) changes.Add($"Streetaddress: {oldValues.Streetaddress} -> {streetAddress}");
            if (oldValues.City != city) changes.Add($"City: {oldValues.City} -> {city}");
            if (oldValues.Zipcode != zipcode) changes.Add($"Zipcode: {oldValues.Zipcode} -> {zipcode}");
            if (oldValues.Country != country) changes.Add($"Country: {oldValues.Country} -> {country}");
            if (oldValues.CountryCode != countryCode) changes.Add($"CountryCode: {oldValues.CountryCode} -> {countryCode}");
            if (oldValues.Telephonecountrycode != telephoneCountryCode) changes.Add($"Telephonecountrycode: {oldValues.Telephonecountrycode} -> {telephoneCountryCode}");
            if (oldValues.Telephonenumber != telephoneNumber) changes.Add($"Telephonenumber: {oldValues.Telephonenumber} -> {telephoneNumber}");
            if (oldValues.Emailaddress != emailaddress) changes.Add($"Emailaddress: {oldValues.Emailaddress} -> {emailaddress}");
            if (oldValues.NationalId != nationalId) changes.Add($"NationalId: {oldValues.NationalId} -> {nationalId}");
            if (oldValues.Birthday != customerFromDb.Birthday) changes.Add($"Birthday: {oldValues.Birthday} -> {customerFromDb.Birthday}");

            if (changes.Count > 0)
            {
                context.Attach(customerFromDb).State = EntityState.Modified;
                await context.SaveChangesAsync();
            }

            return (customerFromDb, changes);
        }


    }
}

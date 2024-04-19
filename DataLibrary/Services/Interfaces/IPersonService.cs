using DataLibrary.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLibrary.Services.Interfaces
{
    public interface IPersonService
    {
        BankAppDataContext GetDbContext();
        Task<Customer> CreateCustomerAsync(Customer customer, Account account, Disposition disposition, Transaction transaction);
        Task DeleteCustomerAsync(int id);
        Task<Customer> GetCustomerAsync(int id);
        Task<Customer?> GetCustomerByEmailAsync(string email);
        Task<bool> IsUniqueCombination(string givenName, string surname, string streetAddress);
        Task<(Customer, Account, Disposition)> CreateCustomerAsync(
            string gender, string givenName, string surname, string streetAddress, string city, string zipcode,
            string country, string countryCode, string emailaddress, string telephoneCountryCode, string telephoneNumber,
            string? nationalId, int birthdayYear, int birthdayMonth, int birthdayDay, string frequency, decimal initialDeposit,
            string dispositionType);
        Task<(Customer, List<string>)> UpdateCustomerAsync(
            int id, string gender, string givenName, string surname, string streetAddress, string city, string zipcode,
            string country, string countryCode, string emailaddress, string telephoneCountryCode, string telephoneNumber,
            string? nationalId, int birthdayYear, int birthdayMonth, int birthdayDay);
    }
}

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
        Task<Customer> CreateCustomerAsync(Customer customer, Account account, Disposition disposition);
        Task DeleteCustomerAsync(int id);
        Task<Customer> GetCustomerAsync(int id);
    }
}

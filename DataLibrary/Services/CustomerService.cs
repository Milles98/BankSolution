using DataLibrary.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLibrary.Services
{
    public class CustomerService
    {
        private readonly BankAppData2Context _context;

        public CustomerService(BankAppData2Context context)
        {
            _context = context;
        }
        public void CustomerDetails()
        {
            // Code here
        }
    }
}

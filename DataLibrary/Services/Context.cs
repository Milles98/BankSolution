using DataLibrary.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLibrary.Services
{
    public class Context
    {
        public BankAppDataContext GetContext()
        {
            return new BankAppDataContext();
        }
    }
}

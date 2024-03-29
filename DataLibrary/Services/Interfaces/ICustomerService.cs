﻿using DataLibrary.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLibrary.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<CustomerAccountViewModel> GetCustomerDetails(int id);
    }
}
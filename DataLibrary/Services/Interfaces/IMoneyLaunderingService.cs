using DataLibrary.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLibrary.Services.Interfaces
{
    public interface IMoneyLaunderingService
    {
        Task<List<Disposition>> GetDispositionsAsync(string country);
        (List<string>, DateOnly) DetectSuspiciousActivity(List<Disposition> dispositions, DateOnly lastRunDate);
        void GenerateReport(List<string> suspiciousUsers, string filePath, string country);
        void SaveLastRunTime(DateOnly lastRunDate, string filePath);
        DateOnly GetLastRunTime(string filePath);
    }
}

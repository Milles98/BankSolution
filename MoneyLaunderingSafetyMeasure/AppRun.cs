using DataLibrary.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyLaunderingSafetyMeasure
{
    public class AppRun
    {
        private readonly IMoneyLaunderingService _moneyLaunderingService;
        public AppRun(IMoneyLaunderingService moneyLaunderingService)
        {
            _moneyLaunderingService = moneyLaunderingService;
        }

        public async Task Run()
        {
            try
            {
                await _moneyLaunderingService.DetectAndReportSuspiciousActivity();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }
}

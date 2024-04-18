using DataLibrary.Data;
using DataLibrary.Services;
using DataLibrary.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        var services = new ServiceCollection();
        services.AddDbContext<BankAppDataContext>(options =>
            options.UseSqlServer("Server=localhost;Database=BankAppData;Trusted_Connection=True;TrustServerCertificate=true;MultipleActiveResultSets=true"));
        services.AddScoped<IMoneyLaunderingService, MoneyLaunderingService>();

        var serviceProvider = services.BuildServiceProvider();

        try
        {
            using var scope = serviceProvider.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IMoneyLaunderingService>();

            await service.DetectAndReportSuspiciousActivity();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
}

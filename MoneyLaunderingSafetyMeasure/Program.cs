using DataLibrary.Data;
using DataLibrary.Services;
using DataLibrary.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MoneyLaunderingSafetyMeasure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        var serviceCollection = new ServiceCollection();
        ConfigureServices(serviceCollection);

        var serviceProvider = serviceCollection.BuildServiceProvider();

        var appRun = serviceProvider.GetService<AppRun>();
        await appRun.Run();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContext<BankAppDataContext>(options =>
            options.UseSqlServer("DefaultConnection"));
        services.AddTransient<IMoneyLaunderingService, MoneyLaunderingService>();
        services.AddTransient<AppRun>();
    }
}

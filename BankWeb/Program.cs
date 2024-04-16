using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DataLibrary.Data;
using DataLibrary.Services.Interfaces;
using DataLibrary.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                       throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<BankAppDataContext>(options =>
    options.UseSqlServer(connectionString));

//builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<BankAppDataContext>();
builder.Services.AddRazorPages();


builder.Services.AddTransient<DataInitializer>();
builder.Services.AddTransient<ISortingService<Transaction>, SortingService<Transaction>>();
//builder.Services.AddTransient<IPaginationService<Transaction>, PaginationService<Transaction>>();
//builder.Services.AddTransient<IPaginationService<Customer>, PaginationService<Customer>>();
//builder.Services.AddTransient<IPaginationService<Account>, PaginationService<Account>>();
builder.Services.AddTransient<ISortingService<Customer>, SortingService<Customer>>();
builder.Services.AddTransient<ISortingService<Account>, SortingService<Account>>();
builder.Services.AddTransient<IBankService, BankService>();
builder.Services.AddTransient<ICustomerService, CustomerService>();
builder.Services.AddTransient<IAccountService, AccountService>();
builder.Services.AddTransient<ITransactionService, TransactionService>();
builder.Services.AddTransient<ICountryService, CountryService>();
builder.Services.AddTransient<IPersonService, PersonService>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddResponseCaching();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetService<DataInitializer>()?.SeedData();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseResponseCaching();

app.Use(async (context, next) =>
{
    await next();

    var logger = ((IApplicationBuilder)app).ApplicationServices.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Response status code: {StatusCode}", context.Response.StatusCode);
    logger.LogInformation("Response headers: {Headers}", context.Response.Headers);
});

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();

app.Run();
using DataLibrary.Services.Interfaces;
using DataLibrary.Services;
using DataLibrary.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<BankAppDataContext>(options =>
    options.UseSqlServer(connectionString));


builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
//builder.Services.AddTransient<IPaginationService<Transaction>, PaginationService<Transaction>>();
builder.Services.AddTransient<ISortingService<Transaction>, SortingService<Transaction>>();
//builder.Services.AddTransient<IPaginationService<Customer>, PaginationService<Customer>>();
builder.Services.AddTransient<ISortingService<Customer>, SortingService<Customer>>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

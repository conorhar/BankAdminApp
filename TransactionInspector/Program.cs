using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SharedThings;
using SharedThings.Data;
using SharedThings.Services.Accounts;
using SharedThings.Services.Customers;
using TransactionInspector.Services;
using TransactionInspector.Services.EmailService;
using TransactionInspector.Services.TransactionInspector;

namespace TransactionInspector
{
    class Program
    {
        private static ServiceProvider _serviceProvider;

        private static void RegisterServices()
        {
            var services = new ServiceCollection();

            services.AddTransient<ITransactionInspector, Services.TransactionInspector.TransactionInspector>();
            services.AddTransient<ICustomerService, CustomerService>();
            services.AddTransient<IAccountService, AccountService>();
            services.AddTransient<IEmailService, MailtrapEmailService>();

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    "Server=localhost;Database=BankAppData;Trusted_Connection=True;MultipleActiveResultSets=true")
            );

            _serviceProvider = services.BuildServiceProvider(true);
        }

        static void Main(string[] args)
        {
            RegisterServices();
            var scope = _serviceProvider.CreateScope();

            scope.ServiceProvider.GetRequiredService<ITransactionInspector>().Run();
        }
    }
}

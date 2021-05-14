using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SharedThings;

namespace Search
{
    class Program
    {
        private static ServiceProvider _serviceProvider;

        private static void RegisterServices()
        {
            var services = new ServiceCollection();

            services.AddTransient<IAzureUpdater, AzureUpdater>();
            services.AddTransient<IAzureSearcher, AzureSearcher>();

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

            scope.ServiceProvider.GetRequiredService<IAzureUpdater>().Run();

            //scope.ServiceProvider.GetRequiredService<IAzureSearcher>().Run();
        }
    }
}

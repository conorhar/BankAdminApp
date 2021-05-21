using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedThings.Data;
using SharedThings.Services.Customers;
using SharedThings.ViewModels;

namespace BankAdminApp.Controllers
{
    public class CountryController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ICustomerService _customerService;

        public CountryController(ApplicationDbContext dbContext, ICustomerService customerService)
        {
            _dbContext = dbContext;
            _customerService = customerService;
        }

        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "country" })]
        [Authorize(Roles = "Admin, Cashier")]
        public IActionResult TopTen(string country)
        {
            var topCustomers = _dbContext.Customers.Include(c => c.Dispositions)
                .ThenInclude(d => d.Account).Where(c => c.Country == country)
                .OrderByDescending(c => c.Dispositions.Sum(d => d.Account.Balance))
                .Take(10).ToList();

            var viewModel = new CountryTopTenViewModel
            {
                CustomerItems = topCustomers.Select(c => new CountryTopTenViewModel.CustomerItem
                {
                    FullName = _customerService.GetFullName(c),
                    City = c.City,
                    Id = c.CustomerId,
                    TotalBalance = c.Dispositions.Sum(d => d.Account.Balance)
                }).ToList()
            };
            viewModel.Country = country;

            return View(viewModel);
        }
    }
}
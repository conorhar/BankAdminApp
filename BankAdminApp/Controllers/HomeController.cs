using SharedThings.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SharedThings;
using SharedThings.Data;
using SharedThings.Services.Customers;
using SharedThings.ViewModels;

namespace BankAdminApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _dbContext;
        private readonly ICustomerService _customerService;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext dbContext, ICustomerService customerService)
        {
            _logger = logger;
            _dbContext = dbContext;
            _customerService = customerService;
        }

        [ResponseCache(Duration = 30, Location = ResponseCacheLocation.Any)]
        public IActionResult Index(int accountId)
        {
            var viewModel = new HomeIndexViewModel
            {
                TotalCustomers = _dbContext.Customers.Count(),
                TotalAccounts = _dbContext.Accounts.Count(),
                TotalBalance = _dbContext.Accounts.Sum(r => r.Balance),
                AccountId = accountId
            };

            var countries = _dbContext.Customers.Select(r => r.Country).Distinct().ToList();

            viewModel.CountryItems.AddRange(
                countries.Select(country => new HomeIndexViewModel.CountryItem
                {
                    Country = country,
                    TotalCustomers = _dbContext.Customers.Count(r => r.Country == country),
                    TotalAccounts = _dbContext.Accounts.Count(r => r.Dispositions.Any(d => d.Customer.Country == country && d.Type == "OWNER")),
                    TotalBalance = _dbContext.Accounts.Where(r => r.Dispositions.Any(d => d.Customer.Country == country && d.Type == "OWNER")).Sum(r => r.Balance)
                }).ToList());

            return View(viewModel);
        }
    }
}

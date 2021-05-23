using SharedThings.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
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
        private readonly SignInManager<IdentityUser> _signInManager;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext dbContext, ICustomerService customerService,
            SignInManager<IdentityUser> signInManager)
        {
            _logger = logger;
            _dbContext = dbContext;
            _customerService = customerService;
            _signInManager = signInManager;
        }

        [ResponseCache(Duration = 30, Location = ResponseCacheLocation.Any)]
        public IActionResult Index(int accountId)
        {
            var viewModel = new HomeIndexViewModel
            {
                TotalCustomers = _dbContext.Customers.Count(),
                TotalAccounts = _dbContext.Accounts.Count(),
                TotalBalance = _customerService.FormatAmount(_dbContext.Accounts.Sum(r => r.Balance)),
                AccountId = accountId
            };

            var countries = _dbContext.Customers.Select(r => new
            {
                Country = r.Country,
                CountryCode = r.CountryCode
            }).Distinct().ToList();

            viewModel.CountryItems.AddRange(
                countries.Select(c => new HomeIndexViewModel.CountryItem
                {
                    CountryCode = c.CountryCode,
                    Country = c.Country,
                    TotalCustomers = _dbContext.Customers.Count(r => r.Country == c.Country),
                    TotalAccounts = _dbContext.Accounts.Count(r => r.Dispositions.Any(d => d.Customer.Country == c.Country && d.Type == "OWNER")),
                    TotalBalance = _customerService.FormatAmount(_dbContext.Accounts.Where(r => r.Dispositions
                                    .Any(d => d.Customer.Country == c.Country && d.Type == "OWNER"))
                                    .Sum(r => r.Balance))
                }).ToList());

            var vectorMapList = viewModel.CountryItems.Select(c =>
                new HomeIndexViewModel.CountryCodeItem
                {
                    Code = c.CountryCode,
                    Value = CalculateValue(c)
                }).ToList();

            viewModel.VectorMapCodesAndValues =  new HtmlString(JsonConvert.SerializeObject(vectorMapList));

            return View(viewModel);
        }

        private int CalculateValue(HomeIndexViewModel.CountryItem c)
        {
            var totalBalance = _dbContext.Accounts.Where(r => r.Dispositions
                    .Any(d => d.Customer.Country == c.Country && d.Type == "OWNER"))
                    .Sum(r => r.Balance);

            return Convert.ToInt32(totalBalance) / 100000;
        }
    }
}

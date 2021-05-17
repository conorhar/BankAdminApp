using SharedThings.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BankAdminApp.ViewModels;
using SharedThings;
using SharedThings.Data;

namespace BankAdminApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _dbContext;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public IActionResult Index(int accountId)
        {
            var viewModel = new HomeIndexViewModel
            {
                Country = "All",
                TotalCustomers = _dbContext.Customers.Count(),
                TotalAccounts = _dbContext.Accounts.Count(),
                TotalBalance = _dbContext.Accounts.Sum(r => r.Balance),
                AccountId = accountId
            };

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

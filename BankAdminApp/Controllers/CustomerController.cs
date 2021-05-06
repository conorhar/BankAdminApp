using System;
using System.Linq;
using BankAdminApp.Data;
using BankAdminApp.Services.Customers;
using BankAdminApp.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BankAdminApp.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ICustomerService _customerService;

        public CustomerController(ApplicationDbContext dbContext, ICustomerService customerService)
        {
            _dbContext = dbContext;
            _customerService = customerService;
        }

        public IActionResult Index(string q, string sortField, string sortOrder)
        {
            if (int.TryParse(q, out int n) && _dbContext.Customers.Any(r => r.CustomerId == n))
            {
                return RedirectToAction("Details", new { id = n});
            }

            if (string.IsNullOrEmpty(sortField)) sortField = "Id";
            if (string.IsNullOrEmpty(sortOrder)) sortOrder = "asc";

            var query = _customerService.BuildQuery(sortField, sortOrder, q);

            var viewModel = new CustomerIndexViewModel
            {
                CustomerItems = query.Select(r => new CustomerIndexViewModel.CustomerItem
                {
                    Id = r.CustomerId,
                    FirstName = r.Givenname,
                    Surname = r.Surname,
                    Address = r.Streetaddress,
                    City = r.City,
                    Birthday = Convert.ToDateTime(r.Birthday).ToString("yyyy-MM-dd")
                }).ToList()
            };

            viewModel.q = q;
            viewModel.SortField = sortField;
            viewModel.SortOrder = sortOrder;
            viewModel.OppositeSortOrder = sortOrder == "asc" ? "desc" : "asc";
            return View(viewModel);
        }

        public IActionResult Details(int id)
        {
            var dbCustomer = _dbContext.Customers.First(r => r.CustomerId == id);

            var viewModel = new CustomerDetailsViewModel
            {
                Birthday = Convert.ToDateTime(dbCustomer.Birthday).ToString("yyyy-MM-dd"),
                Email = dbCustomer.Emailaddress,
                FullAddress = _customerService.GetFullAddress(dbCustomer),
                FullName = _customerService.GetFullName(dbCustomer),
                FullTelephoneNumber = _customerService.GetFullTelephoneNumber(dbCustomer),
                Gender = dbCustomer.Gender,
                Id = dbCustomer.CustomerId,
                NationalId = _customerService.GetNationalIdOutput(dbCustomer),
                
                AccountItems = _customerService.GetAccounts(dbCustomer).Select(r => new CustomerDetailsViewModel.AccountItem
                {
                    AccountNumber = r.AccountId,
                    Balance = r.Balance,
                    CreationDate = r.Created.ToString("yyyy-MM-dd"),
                    Frequency = r.Frequency,
                    AccountOwnership = _customerService.GetAccountOwnershipInfo(r)
                }).ToList()
            };

            viewModel.TotalBalance = viewModel.AccountItems.Sum(r => r.Balance);

            return View(viewModel);
        }
    }
}
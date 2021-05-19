﻿using System;
using System.Linq;
using JW;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedThings.Data;
using SharedThings.Services.Search;
using SharedThings.ViewModels;
using SharedThings.Services.Api;
using SharedThings.Services.Customers;

namespace BankAdminApp.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ICustomerService _customerService;
        private readonly ISearchService _searchService;
        private readonly IApiService _apiService;

        public CustomerController(ApplicationDbContext dbContext, ICustomerService customerService, ISearchService searchService,
            IApiService apiService)
        {
            _dbContext = dbContext;
            _customerService = customerService;
            _searchService = searchService;
            _apiService = apiService;
        }

        [Authorize(Roles = "Admin, Cashier")]
        public IActionResult Index(string q, string sortField, string sortOrder, int page = 1)
        {
            if (int.TryParse(q, out int n) && _dbContext.Customers.Any(r => r.CustomerId == n))
            {
                return RedirectToAction("Details", new { id = n });
            }

            if (string.IsNullOrEmpty(sortField)) sortField = "SortableId";
            if (string.IsNullOrEmpty(sortOrder)) sortOrder = "asc";

            int pageSize = 50;
            
            //var customerSearchModel = _searchService.GetResults(sortField, sortOrder, q, page, pageSize);
            var result = _searchService.GetResults(sortField, sortOrder, q, page, pageSize);

            var viewModel = new CustomerIndexViewModel
            {
                CustomerItems = result.Value.GetResults().Select(r => new CustomerIndexViewModel.CustomerItem
                {
                    Id = r.Document.SortableId,
                    FirstName = r.Document.FirstName,
                    Surname = r.Document.Surname,
                    Address = r.Document.Address,
                    City = r.Document.City,
                    Birthday = Convert.ToDateTime(r.Document.Birthday).ToString("yyyy-MM-dd")
                }).ToList()
            };

            //int totalAmountInCollection = _customerService.GetTotalAmount(q);
            //int totalAmountInCollection = customerSearchModel.TotalCount;
            int totalAmountInCollection = Convert.ToInt32(result.Value.TotalCount);
            int totalPages = (int)Math.Ceiling((double)totalAmountInCollection / pageSize);

            var pager = new Pager(totalAmountInCollection, page, pageSize);

            viewModel.PagerNumbers = pager.Pages;
            viewModel.LastPage = totalPages;
            viewModel.TotalAmount = totalAmountInCollection;
            viewModel.Page = page;
            viewModel.q = q;
            viewModel.SortField = sortField;
            viewModel.SortOrder = sortOrder;
            viewModel.OppositeSortOrder = sortOrder == "asc" ? "desc" : "asc";
            return View(viewModel);
        }

        [Authorize(Roles = "Admin, Cashier")]
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
                
                AccountItems = _customerService.GetAccounts(dbCustomer.CustomerId).Select(r => new CustomerDetailsViewModel.AccountItem
                {
                    AccountNumber = r.AccountId,
                    Balance = r.Balance,
                    CreationDate = r.Created.ToString("yyyy-MM-dd"),
                    Frequency = r.Frequency,
                    AccountOwnership = _customerService.GetAccountOwnershipInfo(dbCustomer.CustomerId, r.AccountId)
                }).ToList()
            };

            viewModel.TotalBalance = viewModel.AccountItems.Sum(r => r.Balance);

            return View(viewModel);
        }

        [Authorize(Roles = "Admin, Cashier")]
        public IActionResult New()
        {
            var viewModel = new CustomerNewViewModel
            {
                AllGenders = _customerService.GetGendersListItems(),
                AllCountries = _customerService.GetCountriesListItems()
            };

            return View(viewModel);
        }

        [Authorize(Roles = "Admin, Cashier")]
        [HttpPost]
        public IActionResult New(CustomerNewViewModel viewModel)
        {

            return View(viewModel);
        }

        [Authorize(Roles = "Admin, Cashier")]
        public IActionResult ChooseCustomer()
        {
            throw new NotImplementedException();
        }

        [Authorize(Roles = "Admin, Cashier")]
        public IActionResult GetApiKey()
        {
            return View();
        }

        public string GetKeyAjax(int id)
        {
            var generatedToken = _apiService.GenerateJSONWebToken(id.ToString());

            return generatedToken;
        }

        [HttpGet]
        public IActionResult ValidateNationalId(string nationalId)
        {
            if (nationalId != null)
            {
                if (int.TryParse(nationalId, out int n) && nationalId.Length is < 10 or > 12)
                    return Json("Must be between 10-12 characters");

                if (nationalId.Count(Char.IsLetter) > 1)
                    return Json("Cannot contain more than one letter");
                
                if (nationalId.Count(Char.IsLetter) == 1 && !Char.IsLetter(nationalId[^1]))
                    return Json("Letter only allowed as last character");

                char ch = '-';
                var dashCount = nationalId.Count(r => (r == ch));
                if (dashCount > 1)
                    return Json("Character '-' is only allowed once");

                if (dashCount == 1 && nationalId.IndexOf("-", StringComparison.Ordinal) != 6)
                    return Json("Required format '123456-7890'");
            }
            
            return Json(true);
        }
    }
}
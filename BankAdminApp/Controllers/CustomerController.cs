using System;
using System.Linq;
using AutoMapper;
using JW;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedThings.Data;
using SharedThings.Models;
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
            if (ModelState.IsValid)
            {
                var customer = new Customer
                {
                    Gender = _customerService.GetGendersListItems()
                        .First(r => r.Value == viewModel.SelectedGenderId.ToString()).Text,
                    Givenname = viewModel.FirstName,
                    Surname = viewModel.Surname,
                    Streetaddress = viewModel.StreetAddress,
                    City = viewModel.City,
                    Zipcode = viewModel.Zipcode,
                    Country = _customerService.GetCountriesListItems()
                        .First(r => r.Value == viewModel.SelectedCountryId.ToString()).Text,
                    Birthday = viewModel.Birthday,
                    Telephonecountrycode = viewModel.TelephoneCountryCode,
                    Telephonenumber = viewModel.TelephoneNumber,
                    Emailaddress = viewModel.EmailAddress
                };

                customer.CountryCode = _customerService.GetCountryCode(customer.Country);
                customer.NationalId = viewModel.NationalId ?? "";

                var account = _customerService.CreateAccount(customer);

                _dbContext.Customers.Add(customer);
                _dbContext.Accounts.Add(account);
                
                _dbContext.SaveChanges();
                _searchService.AddOrUpdateSearchIndex(customer);

                return RedirectToAction("Details", new {id = customer.CustomerId});
            }

            viewModel.AllGenders = _customerService.GetGendersListItems();
            viewModel.AllCountries = _customerService.GetCountriesListItems();
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

        [HttpGet]
        public IActionResult ValidatePhoneCode(string telephoneCountryCode)
        {
            if (!int.TryParse(telephoneCountryCode, out int n))
                return Json("Must be numeric");

            return Json(true);
        }

        [HttpGet]
        public IActionResult ValidatePhoneNumber(string telephoneNumber)
        {
            var withoutSpaces = telephoneNumber.Replace(" ", "");
            withoutSpaces = withoutSpaces.Replace("-", "");

            if (!int.TryParse(withoutSpaces, out int n))
                return Json("Must be numeric");

            if (withoutSpaces.Length < 9)
                return Json("Minimum 9 numbers");

            return Json(true);
        }

        [HttpGet]
        public IActionResult ValidateZipcode(string zipcode)
        {
            var withoutSpaces = zipcode.Replace(" ", "");

            if (!int.TryParse(withoutSpaces, out int n))
                return Json("Must be numeric");

            if (withoutSpaces.Length < 4)
                return Json("Minimum 4 numbers");

            return Json(true);
        }
    }
}
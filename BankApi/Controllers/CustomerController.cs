using System;
using System.Collections.Generic;
using System.Linq;
using BankApi.ViewModels;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using SharedThings;
using SharedThings.Models;
using SharedThings.Services.Customers;

namespace BankApi.Controllers
{
    [EnableCors("AllowAll")]
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ICustomerService _customerService;

        public CustomerController(ApplicationDbContext dbContext, ICustomerService customerService)
        {
            _dbContext = dbContext;
            _customerService = customerService;
        }

        [Route("{id}")]
        [HttpGet]
        public ActionResult<CustomerDetailsViewModel> Details(int id)
        {
            var customer = _dbContext.Customers.FirstOrDefault(e => e.CustomerId == id);
            if (customer == null) return NotFound();

            var model = new CustomerDetailsViewModel
            {
                FullName = _customerService.GetFullName(customer),
                Id = customer.CustomerId,
                FullAddress = _customerService.GetFullAddress(customer),
                Birthday = Convert.ToDateTime(customer.Birthday).ToString("yyyy-MM-dd"),
                Gender = customer.Gender,
                NationalId = _customerService.GetNationalIdOutput(customer),
                FullTelephoneNumber = _customerService.GetFullTelephoneNumber(customer),
                Email = customer.Emailaddress,

                AccountItems = _customerService.GetAccounts(customer.CustomerId).Select(r => new CustomerDetailsViewModel.AccountItem
                {
                    AccountNumber = r.AccountId,
                    Balance = r.Balance,
                    CreationDate = r.Created.ToString("yyyy-MM-dd"),
                    Frequency = r.Frequency,
                    AccountOwnership = _customerService.GetAccountOwnershipInfo(customer.CustomerId, r.AccountId)
                }).ToList()
            };

            model.TotalBalance = model.AccountItems.Sum(r => r.Balance);

            return Ok(model);
        }
    }
}
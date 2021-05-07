using System;
using System.Collections.Generic;
using System.Linq;
using BankAdminApp.Data;
using BankAdminApp.Services.Transactions;
using BankAdminApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BankAdminApp.Controllers
{
    public class TransactionController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ITransactionService _transactionService;

        public TransactionController(ApplicationDbContext dbContext, ITransactionService transactionService)
        {
            _dbContext = dbContext;
            _transactionService = transactionService;
        }

        public IActionResult New()
        {
            return View();
        }
        
        public IActionResult Wizard(string customerId)
        {
            var viewModel = new TransactionWizardViewModel
            {
                AllOperations = _transactionService.GetOperationListItems()
            };
            
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Wizard(TransactionWizardViewModel viewModel)
        {
            viewModel.AllAccounts = _transactionService.GetAccountListItems(viewModel.CustomerId);

            return View(viewModel);
        }

        [HttpPost]
        public JsonResult AutoComplete(string prefix)
        {

            if (int.TryParse(prefix, out int n))
            {
                var customers = (from c in _dbContext.Customers
                    where c.CustomerId == n
                    select new
                    {
                        label = $"{c.Givenname} {c.Surname} (id: {c.CustomerId})",
                        val = c.CustomerId
                    }).ToList();

                return Json(customers);
            }
            else
            {
                var customers = (from c in _dbContext.Customers
                    where c.Givenname.StartsWith(prefix) || c.Surname.StartsWith(prefix)
                    select new
                    {
                        label = $"{c.Givenname} {c.Surname} (id: {c.CustomerId})",
                        val = c.CustomerId
                    }).ToList();

                return Json(customers);
            }
        }

        public int CheckCustomerId(int id)
        {
            if (_dbContext.Customers.Any(r => r.CustomerId == id))
                return 1;

            return 2;
        }
    }
}
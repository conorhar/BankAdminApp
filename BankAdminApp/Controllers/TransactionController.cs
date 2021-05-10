using System;
using System.Collections.Generic;
using System.Linq;
using BankAdminApp.Data;
using BankAdminApp.Models;
using BankAdminApp.Services.Customers;
using BankAdminApp.Services.Transactions;
using BankAdminApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BankAdminApp.Controllers
{
    public class Transact
    {
        public int AccountId { get; set; }
        public string Operation { get; set; }
        public decimal Amount { get; set; }
    }

    public class TransactionController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ITransactionService _transactionService;
        private readonly ICustomerService _customerService;

        public TransactionController(ApplicationDbContext dbContext, ITransactionService transactionService, ICustomerService customerService)
        {
            _dbContext = dbContext;
            _transactionService = transactionService;
            _customerService = customerService;
        }

        public IActionResult ChooseCustomer()
        {
            return View();
        }
        
        public IActionResult ChooseAccountAndOperation(string customerId)
        {
            var dbCustomer = _dbContext.Customers.First(r => r.CustomerId == Convert.ToInt32(customerId));

            var viewModel = new TransactionChooseAccountAndOperationViewModel
            {
                CustomerId = dbCustomer.CustomerId,
                CustomerName = _customerService.GetFullName(dbCustomer),
                AllOperations = _transactionService.GetOperationListItems(),
                AllAccounts = _transactionService.GetAccountListItems(dbCustomer.CustomerId)
            };
            
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult ChooseAccountAndOperation(TransactionChooseAccountAndOperationViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("ChooseAmount", viewModel);
            }

            viewModel.AllAccounts = _transactionService.GetAccountListItems(viewModel.CustomerId);
            viewModel.AllOperations = _transactionService.GetOperationListItems();
            return View(viewModel);
        }
        
        public IActionResult ChooseAmount(TransactionChooseAccountAndOperationViewModel viewModel)
        {
            var currentViewModel = new TransactionChooseAmountViewModel
            {
                AccountId = viewModel.SelectedAccountId,
                Operation = _transactionService.GetOperationString(viewModel.SelectedOperationId),
                CustomerId = viewModel.CustomerId,
                CustomerName = viewModel.CustomerName
            };

            return View(currentViewModel);
        }

        [HttpPost]
        public IActionResult ChooseAmount(TransactionChooseAmountViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("Confirm", viewModel);
            }

            return View(viewModel);
        }

        public IActionResult Confirm(TransactionChooseAmountViewModel viewModel)
        {
            var currentViewModel = new TransactionConfirmViewModel
            {
                CustomerId = viewModel.CustomerId,
                CustomerName = viewModel.CustomerName,
                AccountId = viewModel.AccountId,
                Operation = viewModel.Operation,
                Amount = viewModel.Amount,
                Bank = viewModel.Bank,
                ExternalAccount = viewModel.ExternalAccount,
                CurrentBalance = _dbContext.Accounts.First(r => r.AccountId == viewModel.AccountId).Balance
            };

            return View(currentViewModel);
        }

        [HttpPost]
        public bool MakeTransaction([FromBody] Transact t)
        {
            return true;
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
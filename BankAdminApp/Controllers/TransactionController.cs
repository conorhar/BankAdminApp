using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using SharedThings.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SharedThings;
using SharedThings.Data;
using SharedThings.Services.Accounts;
using SharedThings.Services.Customers;
using SharedThings.Services.Transactions;
using SharedThings.Services.Validation;
using SharedThings.ViewModels;

namespace BankAdminApp.Controllers
{
    public class TransactionController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ITransactionService _transactionService;
        private readonly ICustomerService _customerService;
        private readonly IAccountService _accountService;
        private readonly IValidationService _validationService;

        public TransactionController(ApplicationDbContext dbContext, ITransactionService transactionService, 
            ICustomerService customerService, IAccountService accountService, IValidationService validationService)
        {
            _dbContext = dbContext;
            _transactionService = transactionService;
            _customerService = customerService;
            _accountService = accountService;
            _validationService = validationService;
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
                string operation = _transactionService.GetOperationString(viewModel.SelectedOperationId);

                var nextViewModel = new TransactionChooseAmountViewModel
                {
                    AccountId = viewModel.SelectedAccountId,
                    Operation = operation,
                    CustomerId = viewModel.CustomerId,
                    CustomerName = viewModel.CustomerName,
                    CurrentBalance = _dbContext.Accounts.First(r => r.AccountId == viewModel.SelectedAccountId).Balance,
                    Type = _transactionService.GetType(operation)
                };

                return View("ChooseAmount", nextViewModel);
            }

            viewModel.AllAccounts = _transactionService.GetAccountListItems(viewModel.CustomerId);
            viewModel.AllOperations = _transactionService.GetOperationListItems();
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult ChooseAmount(TransactionChooseAmountViewModel viewModel)
        {
            AddServerValidation(viewModel);

            if (ModelState.IsValid)
            {
                var nextViewModel = new TransactionConfirmViewModel
                {
                    CustomerId = viewModel.CustomerId,
                    CustomerName = viewModel.CustomerName,
                    AccountId = viewModel.AccountId,
                    Operation = viewModel.Operation,
                    Amount = viewModel.Amount,
                    Bank = viewModel.Bank,
                    ExternalAccount = viewModel.ExternalAccount,
                    Type = viewModel.Type,
                    CurrentBalance = viewModel.CurrentBalance,
                    InternalAccountId = viewModel.InternalAccountId
                };

                return View("Confirm", nextViewModel);
            }

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Confirm(TransactionConfirmViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var account = _dbContext.Accounts.First(r => r.AccountId == viewModel.AccountId);
                var transaction = _transactionService.CreateTransaction(viewModel);

                _dbContext.Transactions.Add(transaction);
                account.Balance = transaction.Balance;
                if (transaction.Operation == "Remittance to Internal Account")
                {
                    _dbContext.Transactions.Add(_transactionService.CreateTransactionForReceiver(viewModel));
                }
                _dbContext.SaveChanges();

                return RedirectToAction("Index", "Home", new { accountId = viewModel.AccountId});
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public JsonResult AutoCompleteCustomer(string prefix)
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

        [HttpPost]
        public JsonResult AutoCompleteAccount(string prefix)
        {
            if (int.TryParse(prefix, out int n))
            {
                var accounts = (from a in _dbContext.Accounts
                    where a.AccountId == n
                    select new
                    {
                        label = $"Account id: {a.AccountId} - {_accountService.GetCustomerFullName(a.AccountId)}",
                        val = a.AccountId
                    }).ToList();

                return Json(accounts);
            }
            else return Json("no accounts found");
        }

        public int CheckCustomerId(int id)
        {
            if (_dbContext.Customers.Any(r => r.CustomerId == id))
                return 1;

            return 2;
        }

        [HttpGet]
        public IActionResult ValidateAmount(string amount, int accountId, string type)
        {
            var amtDecimal = Convert.ToDecimal(amount);

            if (_validationService.BalanceIsInsufficient(amtDecimal, accountId, type))
                return Json("Insufficient funds in account");
            
            if (!_validationService.Has2DecimalPlacesOrLess(amtDecimal))
                return Json("Amount cannot have more than two decimal places");

            return Json(true);
        }

        [HttpGet]
        public IActionResult ValidateBankCode(string bank, string operation)
        {
            if (operation == "Remittance to Another Bank")
            {
                if (!string.IsNullOrEmpty(bank) && bank.All(char.IsLetter) && bank.ToUpper() == bank && bank.Length == 2)
                    return Json(true);
                else
                    return Json("Invalid bank code - required: 2 capital letters eg. BA");
            }

            return Json(true);
        }

        [HttpGet]
        public IActionResult ValidateExternalAccount(string externalAccount, string operation)
        {
            if (operation == "Remittance to Another Bank")
            {
                if (int.TryParse(externalAccount, out int n) && externalAccount.Length == 8)
                    return Json(true);

                return Json("Invalid account number - required: 8 digits");
            }

            return Json(true);
        }

        private void AddServerValidation(TransactionChooseAmountViewModel viewModel)
        {
            if (viewModel.Operation == "Remittance to Another Bank")
            {
                if (string.IsNullOrWhiteSpace(viewModel.ExternalAccount))
                    ModelState.AddModelError("ExternalAccount", "Please enter receiver's account number");

                if (string.IsNullOrWhiteSpace(viewModel.Bank))
                    ModelState.AddModelError("Bank", "Please enter receiver's bank code");
            }

            if (viewModel.Operation == "Remittance to Internal Account")
            {
                if (viewModel.InternalAccountId == 0)
                    ModelState.AddModelError("InternalAccountId", "Please choose an account");
            }
        }
    }
}
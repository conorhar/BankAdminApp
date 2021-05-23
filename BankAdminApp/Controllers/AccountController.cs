using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SharedThings;
using SharedThings.Data;
using SharedThings.Services.Accounts;
using SharedThings.Services.Customers;
using SharedThings.ViewModels;

namespace BankAdminApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IAccountService _accountService;
        private readonly ICustomerService _customerService;

        public AccountController(ApplicationDbContext dbContext, IAccountService accountService, ICustomerService customerService)
        {
            _dbContext = dbContext;
            _accountService = accountService;
            _customerService = customerService;
        }

        [Authorize(Roles = "Admin, Cashier")]
        public IActionResult Details(int id)
        {
            var dbTransactions = _accountService.GetTransactionsFrom(id, 0);

            var dbAccount = _dbContext.Accounts.First(r => r.AccountId == id);

            var viewModel = new AccountDetailsViewModel
            {
                AmountClicksUntilEnd = (_accountService.GetTotalAmountTransactions(id) / 20) + 1,
                AccountId = dbAccount.AccountId,
                Balance = _customerService.FormatAmount(dbAccount.Balance),
                TotalTransactions = _accountService.GetTotalAmountTransactions(id),

                TransactionItems = dbTransactions.Select(r => new AccountTransactionRowViewModel()
                {
                    TransactionId = r.TransactionId,
                    Date = r.Date.ToString("yyyy-MM-dd"),
                    Type = r.Type,
                    Amount = _customerService.FormatAmount(r.Amount),
                    Balance = _customerService.FormatAmount(r.Balance),
                    JsonObj = JsonConvert.SerializeObject(r, Formatting.Indented,
                    new JsonSerializerSettings
                    {
                        PreserveReferencesHandling = PreserveReferencesHandling.Objects
                    })
                }).ToList()
            };

            return View(viewModel);
        }

        [Authorize(Roles = "Admin, Cashier")]
        public IActionResult GetTransactionsFrom(int id, int pos)
        {
            var dbTransactions = _accountService.GetTransactionsFrom(id, pos);
            var viewModel = new AccountGetTransactionsFromViewModel();

            if (dbTransactions == null)
            {
                viewModel.ReachedEnd = true;
                return View(viewModel);
            }
            
            viewModel.TransactionItems = dbTransactions.Select(r =>
            new AccountTransactionRowViewModel()
            {
                TransactionId = r.TransactionId,
                Date = r.Date.ToString("yyyy-MM-dd"),
                Type = r.Type,
                Amount = _customerService.FormatAmount(r.Amount),
                Balance = _customerService.FormatAmount(r.Balance),
                JsonObj = JsonConvert.SerializeObject(r, Formatting.Indented,
                    new JsonSerializerSettings
                    {
                        PreserveReferencesHandling = PreserveReferencesHandling.Objects
                    })
            }).ToList();
            

            return View(viewModel);
        }
    }
}
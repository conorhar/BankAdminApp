using System;
using System.Linq;
using BankAdminApp.Data;
using BankAdminApp.Services.Accounts;
using BankAdminApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BankAdminApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IAccountService _accountService;

        public AccountController(ApplicationDbContext dbContext, IAccountService accountService)
        {
            _dbContext = dbContext;
            _accountService = accountService;
        }

        public IActionResult Details(int id)
        {
            var dbTransactions = _accountService.GetTransactionsFrom(id, 0);

            var dbAccount = _dbContext.Accounts.First(r => r.AccountId == id);

            var viewModel = new AccountDetailsViewModel
            {
                AmountClicksUntilEnd = (_accountService.GetTotalAmountTransactions(id) / 20) + 1,
                AccountId = dbAccount.AccountId,
                Balance = dbAccount.Balance,
                TotalTransactions = _accountService.GetTotalAmountTransactions(id),

                TransactionItems = dbTransactions.Select(r => new AccountTransactionRowViewModel()
                {
                    TransactionId = r.TransactionId,
                    Date = r.Date.ToString("yyyy-MM-dd"),
                    Type = r.Type,
                    Amount = r.Amount,
                    Balance = r.Balance,
                    JsonObj = JsonConvert.SerializeObject(r, Formatting.Indented,
                    new JsonSerializerSettings
                    {
                        PreserveReferencesHandling = PreserveReferencesHandling.Objects
                    })
                }).ToList()
            };

            return View(viewModel);
        }

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
                Amount = r.Amount,
                Balance = r.Balance,
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
using System.Linq;
using BankAdminApp.Data;
using BankAdminApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace BankAdminApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public AccountController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Details(int id)
        {
            var dbTransactions = _dbContext.Transactions.Where(r => r.AccountId == id)
                .OrderByDescending(r => r.Date);

            

            var viewModel = new AccountDetailsViewModel
            {
                AccountId = id,

                TransactionItems = dbTransactions.Select(r => new AccountDetailsViewModel.TransactionItem
                {
                    TransactionId = r.TransactionId,
                    Date = r.Date.ToString("yyyy-MM-dd"),
                    Type = r.Type,
                    Amount = r.Amount,
                    Balance = r.Balance,
                    JsonObj = JsonConvert.SerializeObject(r)
                }).ToList()
            };

            return View(viewModel);
        }
    }
}
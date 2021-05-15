using System.Collections.Generic;
using System.Linq;
using BankApi.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedThings;
using SharedThings.Models;

namespace BankApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public AccountController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [Route("id={id}&offset={offset}&limit={limit}")]
        [HttpGet]
        public ActionResult<IEnumerable<Transaction>> GetTransactions(int id, int offset, int limit)
        {
            var account = _dbContext.Accounts.Include(r => r.Transactions).FirstOrDefault(r => r.AccountId == id);
            if (account == null) return NotFound();

            var transactions = account.Transactions;
            if (offset > transactions.Count - 1) return NotFound();

            limit = (limit > 20) ? 20 : limit;
            
            var result = transactions.Skip(offset).Take(limit);

            var model = new AccountGetTransactionsViewModel
            {
                TransactionItems = result.Select(r => new AccountGetTransactionsViewModel.TransactionItem
                {
                    TransactionId = r.TransactionId,
                    AccountId = r.AccountId,
                    Date = r.Date.ToString("yyyy-MM-dd"),
                    Type = r.Type,
                    Operation = r.Operation,
                    Amount = r.Amount,
                    Balance = r.Balance,
                    Symbol = r.Symbol,
                    Bank = r.Bank,
                    Account = r.Account
                }).ToList()
            };
            
            return Ok(model);
        }
    }
}
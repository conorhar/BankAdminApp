using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedThings;
using SharedThings.Data;
using SharedThings.Models;
using SharedThings.ViewModels;
using Swashbuckle.AspNetCore.Annotations;

namespace BankApi.Controllers
{
    [EnableCors("AllowAll")]
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes =
        JwtBearerDefaults.AuthenticationScheme)]
    public class AccountController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public AccountController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [Route("id={id}&offset={offset}&limit={limit}")]
        [HttpGet]
        [SwaggerOperation(OperationId = "GetTransactions")]
        [Authorize(Roles = "Customer")]
        public ActionResult<AccountGetTransactionsApiViewModel> GetTransactions(int id, int offset, int limit)
        {
            var account = _dbContext.Accounts.Include(r => r.Transactions).FirstOrDefault(r => r.AccountId == id);
            if (account == null) return NotFound();

            var transactions = account.Transactions;
            if (offset > transactions.Count - 1) return NotFound();

            limit = (limit > 20) ? 20 : limit;
            
            var result = transactions.Skip(offset).Take(limit);

            var model = new AccountGetTransactionsApiViewModel
            {
                TransactionItems = result.Select(r => new AccountGetTransactionsApiViewModel.TransactionItem
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
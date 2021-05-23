using System.Linq;
using SharedThings.Data;
using SharedThings.Models;
using SharedThings.Services.Customers;

namespace SharedThings.Services.Accounts
{
    public class AccountService : IAccountService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ICustomerService _customerService;

        public AccountService(ApplicationDbContext dbContext, ICustomerService customerService)
        {
            _dbContext = dbContext;
            _customerService = customerService;
        }

        public IQueryable<Transaction> GetTransactionsFrom(int id, int pos)
        {
            var allTransactions = _dbContext.Transactions.Where(r => r.AccountId == id)
                .OrderByDescending(r => r.TransactionId);
            
            var next20 = allTransactions.Skip(pos).Take(20);

            if (allTransactions.Count() < pos)
            {
                return null;
            }

            return next20;
        }

        public int GetTotalAmountTransactions(int id)
        {
            var allTransactions = _dbContext.Transactions.Where(r => r.AccountId == id);

            return allTransactions.Count();
        }

        public string GetCustomerFullName(int accountId)
        {
            var customerId = _dbContext.Dispositions.First(r => r.AccountId == accountId && r.Type == "OWNER").CustomerId;

            return _customerService.GetFullName(_dbContext.Customers.First(r => r.CustomerId == customerId));
        }

        public string GetCustomerFirstName(int accountId)
        {
            var customerId = _dbContext.Dispositions.First(r => r.AccountId == accountId).CustomerId;

            return _dbContext.Customers.First(r => r.CustomerId == customerId).Givenname;
        }

        public string GetCustomerLastName(int accountId)
        {
            var customerId = _dbContext.Dispositions.First(r => r.AccountId == accountId).CustomerId;

            return _dbContext.Customers.First(r => r.CustomerId == customerId).Surname;
        }

        public decimal GetBalance(int accountId)
        {
            return _dbContext.Accounts.First(r => r.AccountId == accountId).Balance;
        }

        public int GetCustomerId(int accountId)
        {
            throw new System.NotImplementedException();
        }
    }
}
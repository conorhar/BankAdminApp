using System.Linq;
using BankAdminApp.Data;

namespace BankAdminApp.Services.Accounts
{
    public class AccountService : IAccountService
    {
        private readonly ApplicationDbContext _dbContext;

        public AccountService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
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
    }
}
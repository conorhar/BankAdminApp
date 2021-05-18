using System;
using System.Linq;
using SharedThings.Data;

namespace SharedThings.Services.Validation
{
    public class ValidationService : IValidationService
    {
        private readonly ApplicationDbContext _dbContext;

        public ValidationService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public bool Has2DecimalPlacesOrLess(decimal amount)
        {
            decimal value = amount * 100;
            return value == Math.Floor(value);
        }

        public bool BalanceIsInsufficient(decimal amount, int accountId, string type)
        {
            var account = _dbContext.Accounts.First(r => r.AccountId == accountId);

            return (type == "Debit" && account.Balance < amount);
        }
    }
}
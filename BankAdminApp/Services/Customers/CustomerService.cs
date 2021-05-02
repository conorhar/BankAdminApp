using System.Collections.Generic;
using System.Linq;
using BankAdminApp.Data;

namespace BankAdminApp.Services.Customers
{
    public class CustomerService : ICustomerService
    {
        private readonly ApplicationDbContext _dbContext;

        public CustomerService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public string GetFullName(Customer c)
        {
            return $"{c.Givenname} {c.Surname}";
        }

        public string GetFullAddress(Customer c)
        {
            return $"{c.Streetaddress}, {c.City}, {c.Zipcode}, {c.Country}";
        }

        public string GetFullTelephoneNumber(Customer c)
        {
            return $"+{c.Telephonecountrycode} {c.Telephonenumber}";
        }

        public string GetNationalIdOutput(Customer c)
        {
            var result = c.NationalId;

            if (string.IsNullOrWhiteSpace(result)) result = "Not registered";

            return result;
        }

        public List<Account> GetAccounts(Customer c)
        {
            var list = new List<Account>();

            var dispositions = _dbContext.Dispositions.Where(r => r.CustomerId == c.CustomerId).ToList();

            foreach (var d in dispositions)
            {
                list.AddRange(_dbContext.Accounts.Where(r => r.AccountId == d.AccountId));
            }

            return list;
        }

        public string GetAccountOwnershipInfo(Account a)
        {
            var disposition = _dbContext.Dispositions.First(r => r.AccountId == a.AccountId);

            return disposition.Type;
        }

        public IQueryable<Customer> GetResults(string q)
        {
            return _dbContext.Customers.Take(50).Where(r =>
                q == null || r.Givenname.Contains(q) || r.Surname.Contains(q)
                || r.City.Contains(q));
        }
    }
}
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

        private IQueryable<Customer> GetResults(string q)
        {
            return _dbContext.Customers.Where(r =>
                q == null || r.Givenname.Contains(q) || r.Surname.Contains(q)
                || r.City.Contains(q));
        }

        public IQueryable<Customer> BuildQuery(string sortField, string sortOrder, string q)
        {
            var query = GetResults(q);

            if (sortField == "Id")
            {
                if (sortOrder == "asc")
                    query = query.OrderBy(r => r.CustomerId);
                else
                    query = query.OrderByDescending(r => r.CustomerId);
            }

            if (sortField == "FirstName")
            {
                if (sortOrder == "asc")
                    query = query.OrderBy(r => r.Givenname);
                else
                    query = query.OrderByDescending(r => r.Givenname);
            }

            if (sortField == "Surname")
            {
                if (sortOrder == "asc")
                    query = query.OrderBy(r => r.Surname);
                else
                    query = query.OrderByDescending(r => r.Surname);
            }

            if (sortField == "Address")
            {
                if (sortOrder == "asc")
                    query = query.OrderBy(r => r.Streetaddress);
                else
                    query = query.OrderByDescending(r => r.Streetaddress);
            }

            if (sortField == "City")
            {
                if (sortOrder == "asc")
                    query = query.OrderBy(r => r.City);
                else
                    query = query.OrderByDescending(r => r.City);
            }

            if (sortField == "Birthday")
            {
                if (sortOrder == "asc")
                    query = query.OrderBy(r => r.Birthday);
                else
                    query = query.OrderByDescending(r => r.Birthday);
            }

            return query;
        }
    }
}
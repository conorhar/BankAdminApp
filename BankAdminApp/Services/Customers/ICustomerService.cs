using System.Collections.Generic;
using System.Linq;
using BankAdminApp.Data;

namespace BankAdminApp.Services.Customers
{
    public interface ICustomerService
    {
        string GetFullName(Customer c);
        string GetFullAddress(Customer c);
        string GetFullTelephoneNumber(Customer c);
        string GetNationalIdOutput(Customer c);
        List<Account> GetAccounts(Customer c);
        string GetAccountOwnershipInfo(Account a);
        IQueryable<Customer> BuildQuery(string sortField, string sortOrder, string q);
    }
}
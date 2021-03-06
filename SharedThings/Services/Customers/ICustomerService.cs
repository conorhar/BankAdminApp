using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using SharedThings.Models;

namespace SharedThings.Services.Customers
{
    public interface ICustomerService
    {
        string GetFullName(Customer c);
        string GetFullAddress(Customer c);
        string GetFullTelephoneNumber(Customer c);
        string GetNationalIdOutput(Customer c);
        List<Account> GetAccounts(int customerId);
        string GetAccountOwnershipInfo(int customerId, int accountId);
        List<SelectListItem> GetGendersListItems(string newOrEdit = null);
        List<SelectListItem> GetCountriesListItems(string newOrEdit = null);
        string GetCountryCode(string customerCountry);
        Account CreateAccount(Customer customer);

        string FormatAmount(decimal balance);
    }
}
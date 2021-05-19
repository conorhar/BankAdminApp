using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SharedThings.Data;
using SharedThings.Models;

namespace SharedThings.Services.Customers
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

        public List<Account> GetAccounts(int customerId)
        {
            var list = new List<Account>();

            var dispositions = _dbContext.Dispositions.Where(r => r.CustomerId == customerId).ToList();

            foreach (var d in dispositions)
            {
                list.AddRange(_dbContext.Accounts.Include(r => r.Transactions)
                    .Where(r => r.AccountId == d.AccountId));
            }

            return list;
        }

        public string GetAccountOwnershipInfo(int customerId, int accountId)
        {
            var disposition = _dbContext.Dispositions.First(r => r.AccountId == accountId && r.CustomerId == customerId);

            return disposition.Type;
        }

        public List<SelectListItem> GetGendersListItems(string newOrEdit)
        {
            var list = new List<SelectListItem>();
            
            if (newOrEdit == null)
                list.Add(new SelectListItem { Value = "0", Text = "Choose an option" });
            
            list.Add(new SelectListItem {Value = "1", Text = "female"});
            list.Add(new SelectListItem {Value = "2", Text = "male"});
            list.Add(new SelectListItem {Value = "3", Text = "other"});

            return list;
        }

        public List<SelectListItem> GetCountriesListItems(string newOrEdit)
        {
            var countries = _dbContext.Customers.Select(r => r.Country).Distinct().ToList();
            var list = new List<SelectListItem>();

            if (newOrEdit == null)
                 list.Add(new SelectListItem {Value = "0", Text = "Choose a country"});

            for (int i = 0; i < countries.Count; i++)
            {
                list.Add(new SelectListItem {Value = (i + 1).ToString(), Text = countries[i]});
            }

            return list;
        }

        public string GetCountryCode(string country)
        {
            switch (country)
            {
                case "Denmark":
                    return "DK";
                case "Finland":
                    return "FI";
                case "Norway":
                    return "NO";
                case "Sweden":
                    return "SE";
            }

            return null;
        }

        public Account CreateAccount(Customer customer)
        {
            var account = new Account
            {
                Frequency = "Monthly",
                Created = DateTime.Now.Date,
                Balance = 0
            };

            var disposition = new Disposition
            {
                Account = account,
                Customer = customer,
                Type = "OWNER"
            };

            customer.Dispositions.Add(disposition);
            account.Dispositions.Add(disposition);

            return account;
        }
        
        //public IQueryable<Customer> BuildQuery(string sortField, string sortOrder, string q, int page, int pageSize)
        //{
        //    var query = GetResults(q);
        //    query = AddSorting(query, sortField, sortOrder);
        //    query = AddPaging(query, page, pageSize);

        //    return query;
        //}

        //private IQueryable<Customer> GetResults(string q)
        //{
        //    return _dbContext.Customers.Where(r =>
        //        q == null || r.Givenname.Contains(q) || r.Surname.Contains(q)
        //        || r.City.Contains(q));
        //}

        //private IQueryable<Customer> AddSorting(IQueryable<Customer> query, string sortField, string sortOrder)
        //{
        //    if (sortField == "Id")
        //    {
        //        if (sortOrder == "asc")
        //            query = query.OrderBy(r => r.CustomerId);
        //        else
        //            query = query.OrderByDescending(r => r.CustomerId);
        //    }

        //    if (sortField == "FirstName")
        //    {
        //        if (sortOrder == "asc")
        //            query = query.OrderBy(r => r.Givenname);
        //        else
        //            query = query.OrderByDescending(r => r.Givenname);
        //    }

        //    if (sortField == "Surname")
        //    {
        //        if (sortOrder == "asc")
        //            query = query.OrderBy(r => r.Surname);
        //        else
        //            query = query.OrderByDescending(r => r.Surname);
        //    }

        //    if (sortField == "Address")
        //    {
        //        if (sortOrder == "asc")
        //            query = query.OrderBy(r => r.Streetaddress);
        //        else
        //            query = query.OrderByDescending(r => r.Streetaddress);
        //    }

        //    if (sortField == "City")
        //    {
        //        if (sortOrder == "asc")
        //            query = query.OrderBy(r => r.City);
        //        else
        //            query = query.OrderByDescending(r => r.City);
        //    }

        //    if (sortField == "Birthday")
        //    {
        //        if (sortOrder == "asc")
        //            query = query.OrderBy(r => r.Birthday);
        //        else
        //            query = query.OrderByDescending(r => r.Birthday);
        //    }

        //    return query;
        //}

        //private IQueryable<Customer> AddPaging(IQueryable<Customer> query, int page, int pageSize)
        //{
        //    int howManyRecordsToSkip = (page - 1) * pageSize;

        //    return query.Skip(howManyRecordsToSkip).Take(pageSize);
        //}

        //public int GetTotalAmount(string q)
        //{
        //    if (q == null)
        //        return _dbContext.Customers.Count();

        //    return GetResults(q).Count();
        //}
    }
}
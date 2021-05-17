using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SharedThings;
using SharedThings.Models;
using SharedThings.Services.Accounts;
using SharedThings.Services.Customers;

namespace TransactionInspector
{
    public interface ITransactionInspector
    {
        void Run();
    }

    public class TransactionInspector : ITransactionInspector
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ICustomerService _customerService;
        private readonly IAccountService _accountService;

        public TransactionInspector(ApplicationDbContext dbContext, ICustomerService customerService, IAccountService accountService)
        {
            _dbContext = dbContext;
            _customerService = customerService;
            _accountService = accountService;
        }

        public void Run()
        {
            var countryList = new List<string> {"Denmark", "Finland", "Norway", "Sweden"};

            foreach (var country in countryList)
            {
                var report = new Report();

                var customers = _dbContext.Customers.Where(r => r.Country == country);

                report = CheckCustomerTransactions(report, customers);
                
                Console.WriteLine($"Report {country}: {report.SuspiciousTransactions.Count} suspicious transactions");
            }

            Console.ReadLine();
        }

        private Report CheckCustomerTransactions(Report report, IQueryable<Customer> customers)
        {
            foreach (var c in customers)
            {
                var accounts = _customerService.GetAccounts(c.CustomerId);

                foreach (var a in accounts)
                {
                    if (_customerService.GetAccountOwnershipInfo(c.CustomerId, a.AccountId) != "OWNER") continue;

                    var transactionsLast72Hours = a.Transactions.Where(r => (DateTime.Now - r.Date).TotalHours <= 72);
                    if (transactionsLast72Hours.Sum(r => r.Amount) > 23000)
                    {
                        report.SuspiciousTransactionGroups.Add(
                            transactionsLast72Hours.Select(r => new Report.ReportItem
                            {
                                CustomerName = _accountService.GetCustomerFullName(r.AccountId),
                                AccountId = r.AccountId,
                                TransactionId = r.TransactionId,
                                Amount = r.Amount
                            }).ToList());
                    }

                    foreach (var t in a.Transactions)
                    {
                        if (t.Amount <= 15000) continue;

                        var reportItem = new Report.ReportItem
                        {
                            CustomerName = _accountService.GetCustomerFullName(t.AccountId),
                            AccountId = t.AccountId,
                            TransactionId = t.TransactionId,
                            Amount = t.Amount
                        };

                        report.SuspiciousTransactions.Add(reportItem);
                    }
                }
            }

            return report;
        }

        //private Report CheckAccounts(Report report, Customer customer)
        //{
            

        //    return report;
        //}

        //private Report CheckTransactions(Report report, Account account)
        //{
            

        //    return report;
        //}
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharedThings;
using SharedThings.Data;
using SharedThings.Models;
using SharedThings.Services.Accounts;
using SharedThings.Services.Customers;
using TransactionInspector.Models;
using TransactionInspector.Services.EmailService;

namespace TransactionInspector.Services.TransactionInspector
{
    public class TransactionInspector : ITransactionInspector
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ICustomerService _customerService;
        private readonly IAccountService _accountService;
        private readonly IEmailService _emailService;

        public TransactionInspector(ApplicationDbContext dbContext, ICustomerService customerService, IAccountService accountService,
            IEmailService emailService)
        {
            _dbContext = dbContext;
            _customerService = customerService;
            _accountService = accountService;
            _emailService = emailService;
        }

        public void Run()
        {
            var countryList = _dbContext.Customers.Select(r => r.Country).Distinct();

            foreach (var country in countryList)
            {
                var report = new Report();

                var customers = _dbContext.Customers.Where(r => r.Country == country);

                report = CheckCustomerTransactions(report, customers);

                var dailyReport = CreateDailyReport(report, country);
                _emailService.SendMail(dailyReport);
            }
        }

        private DailyReport CreateDailyReport(Report report, string country)
        {
            var dailyReport = new DailyReport
            {
                Date = DateTime.Now.AddDays(-1),
                EmailBody = _emailService.CreateEmailBody(report),
                Country = country
            };
            dailyReport.EmailHeader = $"DAILY REPORT {country.ToUpper()} {dailyReport.Date:yyyy-MM-dd}";

            return dailyReport;
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

                    var transactionsLast24Hours = a.Transactions.Where(r => (DateTime.Now - r.Date).TotalHours <= 24);
                    foreach (var t in transactionsLast24Hours)
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
    }
}
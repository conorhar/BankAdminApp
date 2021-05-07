using System.Collections.Generic;
using System.Linq;
using BankAdminApp.Data;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BankAdminApp.Services.Transactions
{
    public class TransactionService : ITransactionService
    {
        private readonly ApplicationDbContext _dbContext;

        public TransactionService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<SelectListItem> GetOperationListItems()
        {
            var list = new List<SelectListItem>();

            list.Add(new SelectListItem { Value = "0", Text = "Choose an operation..." });
            list.Add(new SelectListItem { Value = "1", Text = "Credit" });
            list.Add(new SelectListItem { Value = "2", Text = "Credit in Cash" });
            list.Add(new SelectListItem { Value = "3", Text = "Credit Card Withdrawal" });
            list.Add(new SelectListItem { Value = "4", Text = "Remittance to Another Bank" });
            list.Add(new SelectListItem { Value = "5", Text = "Withdrawal in Cash" });

            return list;
        }

        public List<SelectListItem> GetAccountListItems(int customerId)
        {
            var accountList = new List<Account>();

            var dispositions = _dbContext.Dispositions.Where(r => r.CustomerId == customerId).ToList();

            foreach (var d in dispositions)
            {
                accountList.AddRange(_dbContext.Accounts.Where(r => r.AccountId == d.AccountId));
            }

            var listItems = new List<SelectListItem>();

            listItems.AddRange(accountList.Select(r => new SelectListItem
            {
                Value = r.AccountId.ToString(),
                Text = $"Account number: {r.AccountId} Balance {r.Balance}"
            }));

            return listItems;
        }
    }
}
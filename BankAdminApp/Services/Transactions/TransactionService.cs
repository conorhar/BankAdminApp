using System.Collections.Generic;
using System.Linq;
using BankAdminApp.Data;
using BankAdminApp.Services.Customers;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BankAdminApp.Services.Transactions
{
    public class TransactionService : ITransactionService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ICustomerService _customerService;

        public TransactionService(ApplicationDbContext dbContext, ICustomerService customerService)
        {
            _dbContext = dbContext;
            _customerService = customerService;
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
            var accountList = _customerService.GetAccounts(customerId);

            var listItems = new List<SelectListItem>();

            listItems.Add(new SelectListItem { Value = "0", Text = "Choose an account..." });

            listItems.AddRange(accountList.Select(r => new SelectListItem
            {
                Value = r.AccountId.ToString(),
                Text = $"Account number: {r.AccountId} Balance {r.Balance}"
            }));

            return listItems;
        }

        public string GetOperationString(int selectedOperationId)
        {
            return GetOperationListItems().First(r => r.Value == selectedOperationId.ToString()).Text;
        }
    }
}
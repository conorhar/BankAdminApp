using System;
using System.Collections.Generic;
using System.Linq;
using BankAdminApp.Services.Accounts;
using SharedThings.Models;
using BankAdminApp.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using SharedThings;
using SharedThings.Services.Customers;

namespace BankAdminApp.Services.Transactions
{
    public class TransactionService : ITransactionService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ICustomerService _customerService;
        private readonly IAccountService _accountService;

        public TransactionService(ApplicationDbContext dbContext, ICustomerService customerService, IAccountService accountService)
        {
            _dbContext = dbContext;
            _customerService = customerService;
            _accountService = accountService;
        }

        public List<SelectListItem> GetOperationListItems()
        {
            var list = new List<SelectListItem>();

            list.Add(new SelectListItem { Value = "0", Text = "Choose an operation..." });
            list.Add(new SelectListItem { Value = "1", Text = "Credit" });
            list.Add(new SelectListItem { Value = "2", Text = "Credit in Cash" });
            list.Add(new SelectListItem { Value = "3", Text = "Credit Card Withdrawal" });
            list.Add(new SelectListItem { Value = "4", Text = "Remittance to Another Bank" });
            list.Add(new SelectListItem { Value = "5", Text = "Remittance to Internal Account" });
            list.Add(new SelectListItem { Value = "6", Text = "Withdrawal in Cash" });

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

        public Transaction CreateTransaction(TransactionConfirmViewModel viewModel)
        {
            var transaction = new Transaction
            {
                AccountId = viewModel.AccountId,
                Date = DateTime.Now.Date,
                Type = viewModel.Type,
                Operation = viewModel.Operation,
                Amount = viewModel.Amount,
                Symbol = ""
            };

            if (viewModel.Type == "Credit")
                transaction.Balance = (_accountService.GetBalance(viewModel.AccountId)) + viewModel.Amount;
            else
                transaction.Balance = (_accountService.GetBalance(viewModel.AccountId)) - viewModel.Amount;
            
            if (!string.IsNullOrEmpty(viewModel.Bank))
                transaction.Bank = viewModel.Bank;

            if (!string.IsNullOrEmpty(viewModel.ExternalAccount))
                transaction.Account = viewModel.ExternalAccount;

            if (viewModel.Operation == "Remittance to Internal Account")
            {
                transaction.Bank = "BB";
                transaction.Account = viewModel.InternalAccountId.ToString();
            }

            return transaction;
        }

        public Transaction CreateTransactionForReceiver(TransactionConfirmViewModel viewModel)
        {
            var transaction = new Transaction
            {
                AccountId = viewModel.InternalAccountId,
                Date = DateTime.Now.Date,
                Type = "Credit",
                Operation = "Collection from Internal Account",
                Amount = viewModel.Amount,
                Symbol = "",
                Balance = _dbContext.Accounts.First(r => r.AccountId == Convert.ToInt32(viewModel.InternalAccountId))
                    .Balance += viewModel.Amount,
                Bank = "BB",
                Account = viewModel.AccountId.ToString()
            };

            return transaction;
        }

        public string GetType(string operation)
        {
            if (operation == "Credit" || operation == "Credit in Cash")
                return "Credit";
            else
                return "Debit";
        }
    }
}
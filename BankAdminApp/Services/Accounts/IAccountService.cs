﻿using System.Linq;
using SharedThings.Models;

namespace BankAdminApp.Services.Accounts
{
    public interface IAccountService
    {
        IQueryable<Transaction> GetTransactionsFrom(int id, int pos);
        int GetTotalAmountTransactions(int id);
        string GetCustomerFullName(int accountId);
        string GetCustomerFirstName(int accountId);
        string GetCustomerLastName(int accountId);
        decimal GetBalance(int accountId);
    }
}
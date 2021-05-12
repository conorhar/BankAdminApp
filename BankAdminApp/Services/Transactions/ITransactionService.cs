using System.Collections.Generic;
using BankAdminApp.Data;
using BankAdminApp.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BankAdminApp.Services.Transactions
{
    public interface ITransactionService
    {
        List<SelectListItem> GetOperationListItems();
        List<SelectListItem> GetAccountListItems(int customerId);
        string GetOperationString(int selectedOperationId);
        Transaction CreateTransaction(TransactionConfirmViewModel viewModel);
        string GetType(string operation);
        Transaction CreateTransactionForReceiver(TransactionConfirmViewModel viewModel);
    }
}
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using SharedThings.Models;
using SharedThings.ViewModels;

namespace SharedThings.Services.Transactions
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
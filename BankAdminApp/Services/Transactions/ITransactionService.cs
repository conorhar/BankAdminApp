using System.Collections.Generic;
using BankAdminApp.Data;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BankAdminApp.Services.Transactions
{
    public interface ITransactionService
    {
        List<SelectListItem> GetOperationListItems();
        List<SelectListItem> GetAccountListItems(int customerId);
        string GetOperationString(int selectedOperationId);
    }
}
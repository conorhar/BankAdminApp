using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BankAdminApp.Services.Transactions
{
    public interface ITransactionService
    {
        List<SelectListItem> GetOperationListItems();
        List<SelectListItem> GetAccountListItems(int customerId);
    }
}
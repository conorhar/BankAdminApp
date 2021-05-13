using BankAdminApp.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BankAdminApp.Services.Validation
{
    public interface IValidationService
    {
        bool Has2DecimalPlacesOrLess(decimal amount);
        bool BalanceIsInsufficient(decimal amount, int accountId, string type);
    }
}
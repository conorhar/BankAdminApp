using System.Linq;
using BankAdminApp.Data;

namespace BankAdminApp.Services.Accounts
{
    public interface IAccountService
    {
        IQueryable<Transaction> GetTransactionsFrom(int id, int pos);
        int GetTotalAmountTransactions(int id);
    }
}
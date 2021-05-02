using System.Collections.Generic;

namespace BankAdminApp.ViewModels
{
    public class AccountGetTransactionsFromViewModel
    {
        public bool ReachedEnd { get; set; }

        public List<AccountTransactionRowViewModel> TransactionItems { get; set; }
    }
}
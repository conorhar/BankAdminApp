using System.Collections.Generic;

namespace SharedThings.ViewModels
{
    public class AccountGetTransactionsFromViewModel
    {
        public bool ReachedEnd { get; set; }

        public List<AccountTransactionRowViewModel> TransactionItems { get; set; }
    }
}
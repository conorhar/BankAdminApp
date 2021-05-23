using System.Collections.Generic;

namespace SharedThings.ViewModels
{
    public class AccountDetailsViewModel
    {
        public int AccountId { get; set; }
        public string Balance { get; set; }
        public int AmountClicksUntilEnd { get; set; }
        public List<AccountTransactionRowViewModel> TransactionItems { get; set; }
        public int TotalTransactions { get; set; }
    }
}
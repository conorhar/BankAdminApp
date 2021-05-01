using System.Collections.Generic;

namespace BankAdminApp.ViewModels
{
    public class AccountDetailsViewModel
    {
        public int AccountId { get; set; }
        public List<TransactionItem> TransactionItems { get; set; }

        public class TransactionItem
        {
            public int TransactionId { get; set; }
            public string Date { get; set; }
            public string Type { get; set; }
            public decimal Amount { get; set; }
            public decimal Balance { get; set; }
            public string JsonObj { get; set; }
        }
    }
}
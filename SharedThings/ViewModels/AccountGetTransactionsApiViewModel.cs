using System.Collections.Generic;

namespace SharedThings.ViewModels
{
    public class AccountGetTransactionsApiViewModel
    {
        public List<TransactionItem> TransactionItems { get; set; }

        public class TransactionItem
        {
            public int TransactionId { get; set; }
            public int AccountId { get; set; }
            public string Date { get; set; }
            public string Type { get; set; }
            public string Operation { get; set; }
            public decimal Amount { get; set; }
            public decimal Balance { get; set; }
            public string Symbol { get; set; }
            public string Bank { get; set; }
            public string Account { get; set; }
        }
    }
}
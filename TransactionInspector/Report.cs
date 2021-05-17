using System.Collections.Generic;
using SharedThings.Models;

namespace TransactionInspector
{
    public class Report
    {
        public Country Country { get; set; }
        public int FirstTransactionCheckedId { get; set; }
        public int LastTransactionCheckedId { get; set; }
        public int AmountTransactionsChecked { get; set; }
        public List<ReportItem> SuspiciousTransactions { get; set; } = new List<ReportItem>();
        public List<List<ReportItem>> SuspiciousTransactionGroups { get; set; } = new List<List<ReportItem>>();

        public class ReportItem
        {
            public string CustomerName { get; set; }
            public int AccountId { get; set; }
            public int TransactionId { get; set; }
            public decimal Amount { get; set; } 
        }
    }

    public enum Country
    {
        Denmark,
        Finland,
        Norway,
        Sweden
    }
}
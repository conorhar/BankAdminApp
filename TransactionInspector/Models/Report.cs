using System.Collections.Generic;

namespace TransactionInspector.Models
{
    public class Report
    {
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
}
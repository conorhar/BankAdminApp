using System;
using System.Collections.Generic;

#nullable disable

namespace SharedThings.Models
{
    public partial class PermenentOrder
    {
        public int OrderId { get; set; }
        public int AccountId { get; set; }
        public string BankTo { get; set; }
        public string AccountTo { get; set; }
        public decimal? Amount { get; set; }
        public string Symbol { get; set; }

        public virtual Account Account { get; set; }
    }
}

using System.Collections.Generic;
using System.Security.AccessControl;

namespace SharedThings.ViewModels
{
    public class HomeIndexViewModel
    {
        public int TotalCustomers { get; set; }
        public int TotalAccounts { get; set; }
        public decimal TotalBalance { get; set; }
        public int AccountId { get; set; }

        public List<CountryItem> CountryItems { get; set; } = new List<CountryItem>();

        public class CountryItem
        {
            public string Country { get; set; }
            public int TotalCustomers { get; set; }
            public int TotalAccounts { get; set; }
            public decimal TotalBalance { get; set; }
        }
    }
}
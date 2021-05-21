using System.Collections.Generic;
using System.Security.AccessControl;
using Microsoft.AspNetCore.Html;

namespace SharedThings.ViewModels
{
    public class HomeIndexViewModel
    {
        public int TotalCustomers { get; set; }
        public int TotalAccounts { get; set; }
        public decimal TotalBalance { get; set; }
        public int AccountId { get; set; }
        public HtmlString VectorMapCodesAndValues { get; set; }

        public List<CountryItem> CountryItems { get; set; } = new List<CountryItem>();

        public class CountryItem
        {
            public string CountryCode { get; set; }
            public string Country { get; set; }
            public int TotalCustomers { get; set; }
            public int TotalAccounts { get; set; }
            public decimal TotalBalance { get; set; }
        }

        public class CountryCodeItem
        {
            public string Code { get; set; }
            public int Value { get; set; }
        }
    }
}
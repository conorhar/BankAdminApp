using System.Collections.Generic;

namespace SharedThings.ViewModels
{
    public class CountryTopTenViewModel
    {
        public string Country { get; set; }
        public List<CustomerItem> CustomerItems { get; set; }

        public class CustomerItem
        {
            public int Id { get; set; }
            public string FullName { get; set; }
            public string City { get; set; }
            public decimal TotalBalance { get; set; }
        }
    }
}
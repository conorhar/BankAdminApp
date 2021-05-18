using System.Collections.Generic;

namespace SharedThings.ViewModels
{
    public class CustomerIndexViewModel
    {
        public List<CustomerItem> CustomerItems { get; set; }
        public string SortField { get; set; }
        public string SortOrder { get; set; }
        public string OppositeSortOrder { get; set; }
        public string q { get; set; }
        public int Page { get; set; }
        public int TotalAmount { get; set; }
        public IEnumerable<int> PagerNumbers { get; set; }
        public int LastPage { get; set; }

        public class CustomerItem
        {
            public int Id { get; set; }
            public string FirstName { get; set; }
            public string Surname { get; set; }
            public string Address { get; set; }
            public string City { get; set; }
            public string Birthday { get; set; }
        }
    }
}
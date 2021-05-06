using System.Collections.Generic;

namespace BankAdminApp.ViewModels
{
    public class CustomerIndexViewModel
    {
        public string q { get; set; }
        public List<CustomerItem> CustomerItems { get; set; }

        public class CustomerItem
        {
            public int Id { get; set; }
            public string FullName { get; set; }
            public string Address { get; set; }
            public string City { get; set; }
            public string Birthday { get; set; }
        }
    }
}
using System.Collections.Generic;
using SharedThings.Models;

namespace BankAdminApp.Models
{
    public class CustomerSearchModel
    {
        public int TotalCount { get; set; }
        public List<Customer> Customers { get; set; } = new List<Customer>();
    }
}
using System.Collections.Generic;

namespace BankApi.ViewModels
{
    public class CustomerDetailsViewModel
    {
        public int Id { get; set; }
        public string Gender { get; set; }
        public string FullName { get; set; }
        public string FullAddress { get; set; }
        public string Birthday { get; set; }
        public string NationalId { get; set; }
        public string FullTelephoneNumber { get; set; }
        public string Email { get; set; }
        public decimal TotalBalance { get; set; }

        public List<AccountItem> AccountItems { get; set; }

        public class AccountItem
        {
            public int AccountNumber { get; set; }
            public string Frequency { get; set; }
            public string CreationDate { get; set; }
            public decimal Balance { get; set; }
            public string AccountOwnership { get; set; }
        }
    }
}
using System.Collections.Generic;
using System.Security.AccessControl;

namespace BankAdminApp.ViewModels
{
    public class AccountDetailsViewModel
    {
        public int AccountId { get; set; }
        public decimal Balance { get; set; }
        public int AmountClicksUntilEnd { get; set; }
        public List<AccountTransactionRowViewModel> TransactionItems { get; set; }
    }
}
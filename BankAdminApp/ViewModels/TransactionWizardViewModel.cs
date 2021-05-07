using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BankAdminApp.ViewModels
{
    public class TransactionWizardViewModel
    {
        public int CustomerId { get; set; }
        public int AccountId { get; set; }
        public decimal Amount { get; set; }
        public decimal Balance { get; set; }
        public string Bank { get; set; }
        public string ExternalAccount { get; set; }

        public int SelectedOperationId { get; set; }
        public List<SelectListItem> AllOperations { get; set; }
        public int SelectedAccountId { get; set; }
        public List<SelectListItem> AllAccounts { get; set; }
    }
}
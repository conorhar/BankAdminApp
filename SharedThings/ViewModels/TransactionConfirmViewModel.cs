using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace SharedThings.ViewModels
{
    public class TransactionConfirmViewModel
    {
        public string CustomerName { get; set; }
        public int CustomerId { get; set; }
        public int AccountId { get; set; }
        public string Operation { get; set; }

        [Required]
        [Range(0.01, 100000000, ErrorMessage = "Cannot be negative")]
        [Remote("ValidateAmount", "Transaction", AdditionalFields = "AccountId, Type")]
        public decimal Amount { get; set; }

        [Remote("ValidateBankCode", "Transaction", AdditionalFields = "Operation")]
        public string Bank { get; set; }

        [Remote("ValidateExternalAccount", "Transaction", AdditionalFields = "Operation")]
        public string ExternalAccount { get; set; }
        
        public decimal CurrentBalance { get; set; }
        public string Type { get; set; }
        public int InternalAccountId { get; set; }
    }
}
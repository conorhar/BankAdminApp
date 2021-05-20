using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace SharedThings.ViewModels
{
    public class TransactionChooseAmountViewModel
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public int AccountId { get; set; }
        public string Operation { get; set; }

        [Required(ErrorMessage = "Please enter an amount")]
        [Range(0.01, 100000000, ErrorMessage = "Minimum transaction is 0,01")]
        [Remote("ValidateAmount", "Transaction", AdditionalFields = "AccountId, Type")]
        public decimal Amount { get; set; }

        //[BankCode(ErrorMessage = "Invalid bank code - required: 2 capital letters eg. BA")]
        [Remote("ValidateBankCode", "Transaction", AdditionalFields = "Operation")]
        public string Bank { get; set; }

        //[AccountNumber(ErrorMessage = "Invalid account number - required: 8 digits")]
        [Remote("ValidateExternalAccount", "Transaction", AdditionalFields = "Operation")]
        public string ExternalAccount { get; set; }
        
        public decimal CurrentBalance { get; set; }
        public string Type { get; set; }
        public int InternalAccountId { get; set; }
    }

    public class AccountNumberAttribute : ValidationAttribute, IClientModelValidator
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string accountNumber = Convert.ToString(value);

            if (int.TryParse(accountNumber, out int n) && accountNumber.Length == 8)
                return ValidationResult.Success;

            return new ValidationResult(ErrorMessage);
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            if (!context.Attributes.ContainsKey("data-val"))
                context.Attributes.Add("data-val", "true");
            
            context.Attributes.Add("data-val-validaccountno", ErrorMessage);
        }
    }

    public class BankCodeAttribute : ValidationAttribute, IClientModelValidator
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string bankCode = Convert.ToString(value);

            if (!string.IsNullOrEmpty(bankCode) && bankCode.All(char.IsLetter) && bankCode.ToUpper() == bankCode && bankCode.Length == 2)
                return ValidationResult.Success;

            return new ValidationResult(ErrorMessage);
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            if (!context.Attributes.ContainsKey("data-val"))
                context.Attributes.Add("data-val", "true");

            context.Attributes.Add("data-val-validbankcode", ErrorMessage);
        }
    }
}
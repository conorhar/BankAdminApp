using System.ComponentModel.DataAnnotations;

namespace BankAdminApp.ViewModels
{
    public class TransactionChooseAmountViewModel
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public int AccountId { get; set; }
        public string Operation { get; set; }

        [Required(ErrorMessage = "Please enter an amount")]
        public decimal Amount { get; set; }
        public string Bank { get; set; }
        public string ExternalAccount { get; set; }
        public decimal CurrentBalance { get; set; }
    }
}
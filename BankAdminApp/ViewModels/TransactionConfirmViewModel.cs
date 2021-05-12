namespace BankAdminApp.ViewModels
{
    public class TransactionConfirmViewModel
    {
        public string CustomerName { get; set; }
        public int CustomerId { get; set; }
        public int AccountId { get; set; }
        public string Operation { get; set; }
        public decimal Amount { get; set; }
        public string Bank { get; set; }
        public string ExternalAccount { get; set; }
        public decimal CurrentBalance { get; set; }
        public string Type { get; set; }
        public int InternalAccountId { get; set; }
    }
}
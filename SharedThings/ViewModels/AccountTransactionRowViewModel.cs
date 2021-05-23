namespace SharedThings.ViewModels
{
    public class AccountTransactionRowViewModel
    {
        public int TransactionId { get; set; }
        public string Date { get; set; }
        public string Type { get; set; }
        public string Amount { get; set; }
        public string Balance { get; set; }
        public string JsonObj { get; set; }
    }
}
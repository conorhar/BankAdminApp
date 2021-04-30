using System.Security.AccessControl;

namespace BankAdminApp.ViewModels
{
    public class HomeIndexViewModel
    {
        public string Country { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalAccounts { get; set; }
        public decimal TotalBalance { get; set; }
    }
}
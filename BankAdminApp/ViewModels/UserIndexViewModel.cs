using System.Collections.Generic;

namespace BankAdminApp.ViewModels
{
    public class UserIndexViewModel
    {
        public class UserItem
        {
            public string Id { get; set; }
            public string UserName { get; set; }
            public string Role { get; set; }
        }

        public List<UserItem> Users { get; set; }
    }
}
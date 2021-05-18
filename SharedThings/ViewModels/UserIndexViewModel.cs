using System.Collections.Generic;

namespace SharedThings.ViewModels
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
        public string Username { get; set; }
    }
}
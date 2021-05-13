using System;
using System.Collections.Generic;

#nullable disable

namespace SharedThings.Models
{
    public partial class User
    {
        public int UserId { get; set; }
        public string LoginName { get; set; }
        public byte[] PasswordHash { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}

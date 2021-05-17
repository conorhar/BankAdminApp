using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace SharedThings.Services.Users
{
    public class UserService : IUserService
    {
        private readonly UserManager<IdentityUser> _userManager;

        public UserService(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public string FindRoleName(string userId)
        {
            var user = _userManager.FindByIdAsync(userId).Result;

            var roles = _userManager.GetRolesAsync(user).Result;

            var result = roles.FirstOrDefault();

            if (result == null)
                return "No role assigned";

            return result;
        }
    }
}
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace SharedThings.Data
{
    public class DataInitializer
    {
        public static void SeedData(ApplicationDbContext dbContext, UserManager<IdentityUser> userManager)
        {
            dbContext.Database.Migrate();
            SeedRoles(dbContext);
            SeedUsers(userManager);
        }

        private static void SeedUsers(UserManager<IdentityUser> userManager)
        {
            AddUserIfNotExists(userManager, "stefan.holmberg@systementor.se", "Hejsan123#", new[] { "Admin" });
            AddUserIfNotExists(userManager, "stefan.holmberg@nackademin.se", "Hejsan123#", new[] { "Cashier" });
        }

        private static void AddUserIfNotExists(UserManager<IdentityUser> userManager, string userName, string password, 
            string[] roles)
        {
            if (userManager.FindByEmailAsync(userName).Result != null)
                return;
            var identityUser = new IdentityUser
            {
                UserName = userName,
                Email = userName,
                EmailConfirmed = true
            };
            var result = userManager.CreateAsync(identityUser, password).Result;
            var r = userManager.AddToRolesAsync(identityUser, roles).Result;
        }

        private static void SeedRoles(ApplicationDbContext dbContext)
        {
            var role = dbContext.Roles.FirstOrDefault(r => r.Name == "Admin");
            if (role == null)
                dbContext.Add(new IdentityRole { Name = "Admin", NormalizedName = "Admin" });

            role = dbContext.Roles.FirstOrDefault(r => r.Name == "Cashier");
            if (role == null)
                dbContext.Add(new IdentityRole { Name = "Cashier", NormalizedName = "Cashier" });

            dbContext.SaveChanges();
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using SharedThings.Data;
using SharedThings.ViewModels;

namespace SharedThings.Services.Users
{
    public class UserService : IUserService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _dbContext;

        public UserService(UserManager<IdentityUser> userManager, ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _dbContext = dbContext;
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

        public List<RoleReferenceItem> GetRoleReference()
        {
            var dbRoles = _dbContext.Roles.ToList();
            var referenceList = new List<RoleReferenceItem>();

            for (int i = 0; i < dbRoles.Count(); i++)
            {
                referenceList.Add(new RoleReferenceItem
                {
                    SelectBoxId = (i + 1),
                    DatabaseId = dbRoles[i].Id
                });
            }

            return referenceList;
        }

        public List<SelectListItem> GetRolesListItems()
        {
            var list = new List<SelectListItem>();

            list.Add(new SelectListItem { Value = "0", Text = "No role assigned" });
            var dbRoles = _dbContext.Roles.ToList();

            for (int i = 0; i < dbRoles.Count(); i++)
            {
                list.Add(new SelectListItem
                {
                    Value = (i + 1).ToString(),
                    Text = dbRoles[i].Name
                });
            }
            return list;
        }

        public string GetNewRoleName(UserEditViewModel viewModel)
        {
            var reference = viewModel.ReferenceList.FirstOrDefault(r => r.SelectBoxId == viewModel.SelectedRoleId);
            string newRoleId = "";
            if (reference != null)
            {
                newRoleId = viewModel.ReferenceList.First(r => r.SelectBoxId == viewModel.SelectedRoleId).DatabaseId;
            }

            string newRoleName = newRoleId == "" ? "No role assigned" : _dbContext.Roles.First(r => r.Id == newRoleId).Name;

            return newRoleName;
        }

        public void AssignNewUserToRole(UserNewViewModel viewModel, IdentityUser identityUser)
        {
            var dbRole = viewModel.ReferenceList.FirstOrDefault(r => r.SelectBoxId == viewModel.SelectedRoleId);

            if (dbRole != null)
            {
                var role = _dbContext.Roles.First(r => r.Id == dbRole.DatabaseId);
                var roleResult = _userManager.AddToRoleAsync(identityUser, role.Name).Result;
            }
        }
    }
}
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using SharedThings.ViewModels;

namespace SharedThings.Services.Users
{
    public interface IUserService
    {
        string FindRoleName(string userId);
        List<RoleReferenceItem> GetRoleReference();
        List<SelectListItem> GetRolesListItems();
        string GetNewRoleName(UserEditViewModel viewModel);
        void AssignNewUserToRole(UserNewViewModel viewModel, IdentityUser identityUser);
    }
}
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using SharedThings.ViewModels;

namespace SharedThings.Services.Users
{
    public interface IUserService
    {
        string FindRoleName(string userId);
        List<UserEditViewModel.RoleReferenceItem> GetRoleReference();
        List<SelectListItem> GetRolesListItems();
    }
}
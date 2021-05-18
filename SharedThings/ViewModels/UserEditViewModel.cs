using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SharedThings.ViewModels
{
    public class UserEditViewModel
    {
        public string Id { get; set; }
        public string UserName { get; set; }

        //[Range(1, 10000, ErrorMessage = "Please choose an option")]
        public int SelectedRoleId { get; set; }
        public List<SelectListItem> AllRoles { get; set; } = new List<SelectListItem>();

        public class RoleReferenceItem
        {
            public int SelectBoxId { get; set; }
            public string DatabaseId { get; set; }
        }

        public List<RoleReferenceItem> ReferenceList { get; set; } = new List<RoleReferenceItem>();
    }
}
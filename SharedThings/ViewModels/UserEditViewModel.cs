using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SharedThings.ViewModels
{
    public class UserEditViewModel
    {
        public string Id { get; set; }

        [MaxLength(50)]
        [Required(ErrorMessage = "Please enter a username")]
        [EmailAddress(ErrorMessage = "Please enter a valid email")]
        [Remote("ValidateUsernameIsNew", "User", AdditionalFields = "Id")]
        [DataType(DataType.EmailAddress)]
        public string UserName { get; set; }

        //[Range(1, 10000, ErrorMessage = "Please choose an option")]
        public int SelectedRoleId { get; set; }
        public List<SelectListItem> AllRoles { get; set; } = new List<SelectListItem>();

        public List<RoleReferenceItem> ReferenceList { get; set; } = new List<RoleReferenceItem>();
    }
}
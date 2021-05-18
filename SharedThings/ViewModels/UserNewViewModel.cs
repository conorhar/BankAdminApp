using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SharedThings.ViewModels
{
    public class UserNewViewModel
    {
        [MaxLength(50)]
        [Required(ErrorMessage = "Please enter username")]
        [EmailAddress(ErrorMessage = "Please enter a valid email")]
        [Remote("ValidateUsernameIsNew", "User")]
        [DataType(DataType.EmailAddress)]
        public string UserName { get; set; }

        [RegularExpression("^(?=.*[A-Z])(?=.*[!@#$&*__+.])(?=.*[0-9]).{6,}$", ErrorMessage = "Requirements: 6-20 characters, min 1 uppercase letter, 1 number and 1 symbol")]
        [Required(ErrorMessage = "Please enter password")]
        [StringLength(20, ErrorMessage = "Must have between 6-20 characters")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please confirm password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string PasswordConfirm { get; set; }


        //[Range(1, 10000, ErrorMessage = "Välj en roll")]
        public int SelectedRoleId { get; set; }
        public List<SelectListItem> AllRoles { get; set; } = new List<SelectListItem>();

        public List<RoleReferenceItem> ReferenceList { get; set; } = new List<RoleReferenceItem>();
    }
}
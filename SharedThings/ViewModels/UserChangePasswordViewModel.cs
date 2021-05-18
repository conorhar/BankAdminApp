using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace SharedThings.ViewModels
{
    public class UserChangePasswordViewModel
    {
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [RegularExpression("^(?=.*[A-Z])(?=.*[!@#$&*__+.])(?=.*[0-9]).{6,}$", ErrorMessage = "Requirements: 6-20 characters, min 1 uppercase letter, 1 number and 1 symbol")]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
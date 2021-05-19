using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SharedThings.ViewModels
{
    public class CustomerEditViewModel
    {
        public int Id { get; set; }
        
        public int SelectedGenderId { get; set; }
        public List<SelectListItem> AllGenders { get; set; } = new List<SelectListItem>();

        [MaxLength(50)]
        [Required(ErrorMessage = "Please enter first name")]
        public string FirstName { get; set; }

        [MaxLength(50)]
        [Required(ErrorMessage = "Please enter surname")]
        public string Surname { get; set; }

        [MaxLength(250)]
        [Required(ErrorMessage = "Please enter street address")]
        public string StreetAddress { get; set; }

        [MaxLength(50)]
        [Required(ErrorMessage = "Please enter city")]
        public string City { get; set; }

        [Remote("ValidateZipcode", "Customer")]
        [MaxLength(6)]
        [Required(ErrorMessage = "Please enter zipcode")]
        [DataType(DataType.PostalCode)]
        public string Zipcode { get; set; }
        
        public int SelectedCountryId { get; set; }
        public List<SelectListItem> AllCountries { get; set; } = new List<SelectListItem>();

        [DataType(DataType.Date)]
        public DateTime Birthday { get; set; }

        [Remote("ValidateNationalId", "Customer")]
        [MaxLength(12)]
        public string NationalId { get; set; }

        [Remote("ValidatePhoneCode", "Customer")]
        [MinLength(2, ErrorMessage = "Minimum two numbers")]
        [MaxLength(3)]
        [Required(ErrorMessage = "Please enter country code")]
        [DataType(DataType.PhoneNumber)]
        public string TelephoneCountryCode { get; set; }

        [Remote("ValidatePhoneNumber", "Customer")]
        [MaxLength(12)]
        [Required(ErrorMessage = "Please enter phone number")]
        [DataType(DataType.PhoneNumber)]
        public string TelephoneNumber { get; set; }

        [EmailAddress(ErrorMessage = "Please enter a valid email")]
        [Required(ErrorMessage = "Please enter email address")]
        [DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }
    }
}
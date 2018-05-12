using System;
using System.ComponentModel.DataAnnotations;

namespace Diploma.Models.ViewModels
{
    public class AccountDetailsViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "First Name")]
        [Required(AllowEmptyStrings = false)]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [Required(AllowEmptyStrings = false)]
        public string LastName { get; set; }

        [Display(Name = "Birthday Date")]
        [DataType(DataType.Date)]
        public DateTime BirthdayDate { get; set; }

        [Display(Name = "Facebook Profile Link")]
        [Required(AllowEmptyStrings = false)]
        public string FacebookProfileLink { get; set; }

        [Display(Name = "Linkedin Profile Link")]
        [Required(AllowEmptyStrings = false)]
        public string LinkedinProfileLink { get; set; }

        [Display(Name = "Account Verified")]
        public bool IsAccountVerified { get; set; }

        [Display(Name = "Phone Number")]
        [Phone]
        public string PhoneNumber { get; set; }

        [Display(Name = "Phone Confirmed")]
        public bool PhoneNumberConfirmed { get; set; }

        [Display(Name = "Email Confirmed")]
        public bool EmailConfirmed { get; set; }
    }
}
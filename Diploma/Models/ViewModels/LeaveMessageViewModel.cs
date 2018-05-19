using System.ComponentModel.DataAnnotations;

namespace Diploma.Models.ViewModels
{
    public class LeaveMessageViewModel
    {
        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        public string PhoneNumber { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Message { get; set; }

        public string UserId { get; set; }
    }
}
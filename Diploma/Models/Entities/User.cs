using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Diploma.Models.Entities
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        [DataType(DataType.Date)]
        public DateTime BirthdayDate { get; set; }

        public bool IsAccountVerified { get; set; }

        public string FacebookProfileLink { get; set; }

        public string LinkedinProfileLink { get; set; }

        public virtual List<UserMessage> UserMessages { get; set; }
    }
}
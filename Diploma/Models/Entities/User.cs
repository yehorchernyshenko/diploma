using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Diploma.Models.Entities
{
    public class User : IdentityUser
    {
        public DateTime BirthdayDate { get; set; }

        public bool IsAccountVerified { get; set; }

        public string FacebookProfileLink { get; set; }

        public string LinkedinProfileLink { get; set; }
    }
}
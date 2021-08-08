using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace GeniiApp.Areas.Identity.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [PersonalData]
        [Column(TypeName = "nvarchar(100)")]
        public string FirstName { get; set; }

        [Required]
        [PersonalData]
        [Column(TypeName = "nvarchar(100)")]
        public string SurName { get; set; }
    }
}

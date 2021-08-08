using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeniiApp.Areas.Identity.Data
{
    public class UserViewModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string SurName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }
}

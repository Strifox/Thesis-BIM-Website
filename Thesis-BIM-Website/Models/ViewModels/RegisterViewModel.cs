using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Thesis_BIM_Website.Models.ViewModels
{
    public class RegisterViewModel
    {
        [DisplayName("Username")]
        public string Username { get; set; }
        [DataType(DataType.Password)]
        [DisplayName("Password")]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [DisplayName("Confirm Password")]
        public string ConfirmPassword { get; set; }
        [DisplayName("Email")]
        public string Email { get; set; }
    }
}

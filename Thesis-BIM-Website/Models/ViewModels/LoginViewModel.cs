using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Thesis_BIM_Website.Models
{
    public class LoginViewModel
    {
        [DisplayName("Username/e-mail")]
        [Required(ErrorMessage ="This field is required.")]
        public string Username { get; set; }
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "This field is required.")]
        [DisplayName("Password")]
        public string Password { get; set; }
    }
}

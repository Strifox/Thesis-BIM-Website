using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Thesis_BIM_Website.Data
{
    public class User : IdentityUser
    {
        public List<Invoice> Invoices { get; set; }
        public string Role { get; set; }
        [NotMapped]
        public string Password { get; set; }
        public string ExpoPushToken { get; set; }
    }
}

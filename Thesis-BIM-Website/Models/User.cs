using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Thesis_BIM_Website.Models
{
    public class User: IdentityUser
    {
        public List<Invoice> Invoices { get; set; }
        public string Role { get; set; }
        [NotMapped]
        public string Password { get; set; }
        public string ExpoPushToken { get; set; }
    }
}

using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Thesis_BIM_Website.Models
{
    public class User: IdentityUser
    {
        public virtual List<Invoice> Invoices { get; set; }
        public string Username { get; set; }
        [NotMapped]
        public string Password { get; set; }
        public string ExpoPushToken { get; set; }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Thesis_BIM_Website.Models
{
    public class User: IdentityUser
    {
        public List<Invoice> Invoices { get; set; }
        public string Role { get; set; }
    }
}

using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Thesis_BIM_Website.Models
{
    public class Invoice
    {

        public virtual IdentityUser User { get; set; }

        public string UserId { get; set; }
        public string Id { get; set; }
        public string CompanyName { get; set; }
        public decimal AmountToPay { get; set; }
        public int BankAccountNumber { get; set; }
        public int OCRNumber { get; set; }
        public DateTime Paydate { get; set; }
    }
}

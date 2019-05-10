using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Thesis_BIM_Website.Models
{
    public class Invoice
    {
        public virtual User user { get; set; }
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public decimal AmountToPay { get; set; }
        public string BankAccountNumber { get; set; }
        public long OCRNumber { get; set; }
        public DateTime Paydate { get; set; }

    }
}

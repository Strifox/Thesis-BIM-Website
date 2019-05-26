using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace Thesis_BIM_Website.Models
{
    public class Invoice
    {
        public virtual User User { get; set; }
        public string UserId { get; set; }
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public decimal AmountToPay { get; set; }
        public string BankAccountNumber { get; set; }
        public long Ocr { get; set; }
        public DateTime Paydate { get; set; }

    }
}

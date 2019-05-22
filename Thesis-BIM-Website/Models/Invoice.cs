using System;

namespace Thesis_BIM_Website.Models
{
    public class Invoice
    {
        public User User { get; set; }
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public decimal AmountToPay { get; set; }
        public string BankAccountNumber { get; set; }
        public long Ocr { get; set; }
        public DateTime Paydate { get; set; }

    }
}

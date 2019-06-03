using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Thesis_BIM_Website.Models
{
    public class Invoice
    {
        [NotMapped]
        public string Base64String { get; set; }
        public virtual User User { get; set; }
        public string UserId { get; set; }
        public int Id { get; set; }
        [Display(Name = "Company")]
        public string CompanyName { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:N2}")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "money")]
        [Display(Name = "Amount")]
        public decimal AmountToPay { get; set; }
        [Display(Name = "Bankaccount number")]
        public string BankAccountNumber { get; set; }
        [Display(Name = "OCR Number")]
        public long Ocr { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "Paydate")]
        public DateTime? Paydate { get; set; }

    }
}

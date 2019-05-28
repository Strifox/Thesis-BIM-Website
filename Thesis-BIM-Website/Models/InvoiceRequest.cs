using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Thesis_BIM_Website.Models
{
    public class InvoiceRequest
    {
        [Required]
        [JsonProperty("userId")]
        public string UserId { get; set; }

    }
}

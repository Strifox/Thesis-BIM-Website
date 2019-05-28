using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Thesis_BIM_Website.Models;

namespace Thesis_BIM_Website.Interfaces
{
    public interface IInvoiceService
    {
        bool IsAuthenticated(InvoiceRequest request, out string token);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Thesis_BIM_Website.Interfaces
{
    //Service layer pattern for the database connectivity logic.
    public interface IInvoiceManagementService
    {
        bool IsValidUserId(string userId);
    }
}

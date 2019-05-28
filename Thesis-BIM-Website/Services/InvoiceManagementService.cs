using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Thesis_BIM_Website.Data;
using Thesis_BIM_Website.Interfaces;

namespace Thesis_BIM_Website.Services
{
    //Service layer pattern for the database connectivity logic.
    //Should handle the database connectivity logic ONLY.
    public class InvoiceManagementService : IInvoiceManagementService
    {
        private readonly ApplicationDbContext _context;
        public InvoiceManagementService(ApplicationDbContext context)
        {
            _context = context;
        }
        public bool IsValidUserId(string userId)
        {
            var users = _context.Users.ToList();
            foreach (var user in users)
            {
                if (user.Id == userId)
                    return true;
            }

            return false;
        }
    }
}

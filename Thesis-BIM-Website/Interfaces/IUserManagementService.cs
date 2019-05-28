using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Thesis_BIM_Website.Interfaces
{
    public interface IUserManagementService
    {
        bool IsValidUser(string username, string password);
    }
}

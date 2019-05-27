using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Thesis_BIM_Website.Models
{
    public interface IAuthenticateService
    {
        bool IsAuthenticated(TokenRequest request, out string token);
    }
}

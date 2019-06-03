using Microsoft.AspNetCore.Identity;
using Thesis_BIM_Website.Interfaces;

namespace Thesis_BIM_Website.Services
{
    public class UserManagementService : IUserManagementService
    {
        private readonly UserManager<IdentityUser> _userManager;

        public UserManagementService(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }
        public bool IsValidUser(string username, string password)
        {
            IdentityUser user = _userManager.FindByNameAsync(username).Result;

            if (_userManager.CheckPasswordAsync(user, password).Result)
            {
                return true;
            }

            return false;
        }
    }
}

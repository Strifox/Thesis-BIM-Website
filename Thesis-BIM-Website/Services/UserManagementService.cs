using Microsoft.AspNetCore.Identity;
using Thesis_BIM_Website.Interfaces;
using Thesis_BIM_Website.Models;

namespace Thesis_BIM_Website.Services
{
    public class UserManagementService : IUserManagementService
    {
        private readonly UserManager<User> _userManager;

        public UserManagementService(UserManager<User> userManager)
        {
            _userManager = userManager;
        }
        public bool IsValidUser(string username, string password)
        {
            User user = _userManager.FindByNameAsync(username).Result;

            if (_userManager.CheckPasswordAsync(user, password).Result)
            {
                return true;
            }

            return false;
        }
    }
}

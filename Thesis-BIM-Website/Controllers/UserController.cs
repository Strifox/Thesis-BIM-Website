using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Thesis_BIM_Website.Data;

namespace Thesis_BIM_Website.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;


        public UserController(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        /// <summary>
        /// Creates a user
        /// </summary>
        /// <returns></returns>
        [Route("createUser")]
        [HttpGet]
        public async Task<ActionResult<string>> CreateUser(string username, string password, string email, string pushToken)
        {
            IdentityResult roleResult;
            var roleCheck = await _roleManager.RoleExistsAsync("User");
            if (!roleCheck)
            {
                //create the roles and seed them to the database
                roleResult = await _roleManager.CreateAsync(new IdentityRole("User"));
            }

            if (_context.Users.Any(x => x.UserName == username))
            {
                return Conflict(new { message = "There is already a user registered with that username" });
            }
            if (_context.Users.Any(x => x.Email == email))
            {
                return Conflict(new { message = "There is already a user registered with that email" });
            }

            var user = new User { UserName = username, Email = email, ExpoPushToken = pushToken };

            var result = await _userManager.CreateAsync(user
            , password);
            var setRole = await _userManager.AddToRoleAsync(user, "User");

            if (result.Succeeded)
                return CreatedAtAction("CreateUser", new { user.Id, user.UserName, user.Email });

            return Content("User creation failed");
        }
    }
}

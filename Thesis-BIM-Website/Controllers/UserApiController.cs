using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Thesis_BIM_Website.Data;
using Thesis_BIM_Website.Models;

namespace Thesis_BIM_Website.Controllers
{
    [Route("api/[controller]")]
    [AllowAnonymous]
    [ApiController]
    public class UserApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IAuthenticateService _authService;

        public UserApiController(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IAuthenticateService authService)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _authService = authService;
        }

        /// <summary>
        /// Creates a user
        /// </summary>
        /// <returns></returns>
        [Route("Register")]
        [HttpPost]
        public async Task<ActionResult<string>> Register([FromBody] User request)
        {
            //IdentityResult roleResult;
            //var roleCheck = await _roleManager.RoleExistsAsync("User");
            //if (!roleCheck)
            //{
            //    //create the roles and seed them to the database
            //    roleResult = await _roleManager.CreateAsync(new IdentityRole("User"));
            //}

            if (_context.Users.Any(x => x.UserName == request.UserName))
            {
                return Conflict(new { message = "There is already a user registered with that username" });
            }
            if (_context.Users.Any(x => x.Email == request.Email))
            {
                return Conflict(new { message = "There is already a user registered with that email" });
            }

            var user = new User { UserName = request.UserName, Email = request.Email, ExpoPushToken = request.ExpoPushToken };

            var result = await _userManager.CreateAsync(user
            , request.Password);

            if (result.Succeeded)
                return CreatedAtAction("CreateUser", new { message = "User created" });

            return BadRequest(new { message = "User creation failed" });
        }

        /// <summary>
        /// Gets the user when you login
        /// </summary>
        /// <returns></returns>
        [Route("Login")]
        [HttpPost]
        [AllowAnonymous]
        [HttpPost]
        public ActionResult RequestToken([FromBody] TokenRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string token;
            if (_authService.IsAuthenticated(request, out token))
            {
                return Ok(token);
            }

            return BadRequest("Invalid Request");
        }
    }
}

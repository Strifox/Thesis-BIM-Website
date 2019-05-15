using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Thesis_BIM_Website.Models;
using Thesis_BIM_Website.Models.ViewModels;

namespace Thesis_BIM_Website.Controllers
{
    public class AccountController : Controller, IAuthorizationFilter
    {
        private static HttpClient client = new HttpClient();
        // GET: Account
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult<string> GetUser()
        {
            var userClaim = User.Claims.FirstOrDefault(x => x.Type.Equals("id", StringComparison.InvariantCultureIgnoreCase));

            if (userClaim != null)
            {
                return Ok($"This is your User Id: {userClaim.Value}");
            }
            return BadRequest("No Claim");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {

            ViewData["ReturnUrl"] = returnUrl;

            var token = GetJwtClaim(await GetJwtToken(model.Username, model.Password));
            if (token.Claims == null)
            {
                return View(model);
            }
            var user = new User
            {
                Id = token.Claims.First(x => x.Type == "nameid").Value,
                UserName = token.Claims.First(x => x.Type == "given_name").Value,
                Email = token.Claims.First(x => x.Type == "email").Value,
                Role = token.Claims.First(x => x.Type == "Role").Value,
            };


            return RedirectToLocal(returnUrl);
        }


        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {

            client.BaseAddress = new Uri($"http://localhost:56171");
            var result = await client.GetAsync($"/api/User/createUser?username={model.Username}&password={model.Password}&email={model.Email}");
            string resultContent = await result.Content.ReadAsStringAsync();

            return View(model);
        }


        public async Task<string> GetJwtToken(string username, string password)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri($"http://localhost:56171");
                var result = await client.GetAsync($"/api/User/Login?username={username}&password={password}");
                string resultContent = await result.Content.ReadAsStringAsync();
                var json = JsonConvert.DeserializeObject<Dictionary<string, string>>(resultContent);
                var decodedToken = json.First().Value;

                return decodedToken;
            }
        }

        /// <summary>
        /// Gets decoded token and return claimtype
        /// </summary>
        /// <param name="decodedtoken">Decoded jwttoken</param>
        /// <param name="claimType">Type the claim type you want to return</param>
        /// <returns></returns>
        public JwtSecurityToken GetJwtClaim(string decodedtoken)
        {
            var handler = new JwtSecurityTokenHandler();
            return handler.ReadJwtToken(decodedtoken);
        }


        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }



        // GET: Account/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Account/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Account/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Account/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Account/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Account/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Account/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            throw new NotImplementedException();
        }
    }
}
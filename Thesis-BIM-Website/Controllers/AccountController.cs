using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Thesis_BIM_Website.Models;
using Thesis_BIM_Website.Models.ViewModels;

namespace Thesis_BIM_Website.Controllers
{
    public class AccountController : Controller
    {
        private static readonly HttpClient client = new HttpClient();

        // GET: Account
        public ActionResult Index()
        {
            return View();
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
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri($"http://localhost:56171");
                var result = await client.GetAsync($"/api/User/Login?username={model.Username}&password={model.Password}");
                string resultContent = await result.Content.ReadAsStringAsync();
                var json = JsonConvert.DeserializeObject<User>(resultContent);
                User user = new User { Id = json.Id, UserName = json.UserName, Email = json.Email };
                await Response.WriteAsync(user.ToString());

            }



            return View(model);

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
    }
}
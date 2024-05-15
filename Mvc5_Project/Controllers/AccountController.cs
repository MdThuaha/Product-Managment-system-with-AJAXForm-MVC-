using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mvc5_Project.ViewModels;

namespace Mvc5_Project.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var store = new UserStore<IdentityUser>();
                var manager = new UserManager<IdentityUser>(store);
                var User = manager.Find(model.UserName, model.Password);
                if (User != null)
                {
                    var authManager = HttpContext.GetOwinContext().Authentication;
                    var identity = manager.CreateIdentity(User, DefaultAuthenticationTypes.ApplicationCookie);
                    authManager.SignIn(new AuthenticationProperties { IsPersistent = false }, identity);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", " Login Failed, Please Check Username or Password.");
                }
            }
            return View(model);
        }
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var store = new UserStore<IdentityUser>();
                var manager = new UserManager<IdentityUser>(store);
                var User = new IdentityUser { UserName = model.UserName };
                var result = manager.Create(User, model.Password);

                if (result.Succeeded)
                {
                    manager.AddToRole(User.Id, "Customers");
                    return RedirectToAction("Login");
                }
                else
                {
                    ModelState.AddModelError("", "Registration failed");
                }
            }

            return View(model);
        }
        public ActionResult Logout()
        {
            var authManager = HttpContext.GetOwinContext().Authentication;
            authManager.SignOut();
            return RedirectToAction("Login", "Account");
        }
    }
}
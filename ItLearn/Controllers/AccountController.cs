using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ItLearn.Models;

namespace ItLearn.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(RegistrationModel register)
        {
            var database = DatabaseConnection.GetInstance();
            if (database.GetUserInfo(register.Name, register.Email) == null)
            {
                database.AddUser(register.Name, false,
                                register.Email,
                                register.Password,
                                "blablablabla",
                                register.Country);
                Session["UserName"] = register.Name;
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public ActionResult Login(UserLogin loginData)
        {
            var user = DatabaseConnection.GetInstance().Login(loginData.Name, loginData.Password);

            if (user != null)
            {
                Session["UserName"] = loginData.Name;
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", "User name or password in wrong");
            }
            return View();
        }

        public ActionResult Logout()
        {
            Session["UserName"] = null;
            return RedirectToAction("Index", "Home");
        }
    }
}
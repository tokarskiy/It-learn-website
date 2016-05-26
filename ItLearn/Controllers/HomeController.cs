using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ItLearn.Models;

namespace ItLearn.Controllers
{
    public class HomeController : Controller
    {
        /// <summary>
        ///     The home page controller 
        /// </summary>
        /// <returns>Home page view</returns>
        public ActionResult Index()
        {
            var summaries = DatabaseConnection.GetInstance().GetMenuArticles(6);
            return View(summaries);
        }
    }
}
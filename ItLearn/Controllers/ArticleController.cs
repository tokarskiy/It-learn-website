using ItLearn.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ItLearn.Controllers
{
    public class ArticleController : Controller
    {
        /// <summary>
        ///     Article controller
        /// </summary>
        /// <param name="id">The article name</param>
        /// <returns>The article view</returns>
        public ActionResult Read(string id)
        {
            var newId = id.Replace(@"'", @"\'");
            var article = DatabaseConnection.GetInstance().GetArticle(newId);
            article = article ?? new Article { Name = "{empty}", Contents = "{empty}", Author = "{empty}" };
            
            return View(article);
        }

        public ActionResult Write()
        {
            return View();
        }

        /// <summary>
        ///     Controller of writing article page
        /// </summary>
        /// <param name="article">An article user wanted to create</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Write(Article article)
        {
            if (Session["UserName"] == null)
            {
                return View();
            }
            article.Author = Session["UserName"].ToString();
            DatabaseConnection.GetInstance().AddArticle(article.Name, 
                        new List<string>(), article.Summary, article.Contents, Session["UserName"].ToString());         
            return View();
        }

    }
}
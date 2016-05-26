using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ItLearn.Models
{
    public class Article
    {
        public string Name { get; set; }
        public string Contents { get; set; }
        public string Summary { get; set; }
        public string Author { get; set; }

        public List<string> Tags { get; set; }

        public Article(){
            Tags = new List<string>();
        }

        public string Href(UrlHelper helper)
        {
            return helper.RouteUrl(new { controller = "Article", action = "Read", title = Name });
        }
    }

    public class ArticlePreview
    {
        public string Name { get; set; }
        public string Summary { get; set; }
    }
}
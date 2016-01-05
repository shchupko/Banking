using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace Banking.Controllers
{
    public class HomeController : Controller
    {
        public string FileText = string.Empty;

        public ActionResult Index()
        {
            Logger.Log.Debug("HomeController.Index()");

            string filepath = Server.MapPath("~/Content/Requirements.txt");

            try
            {
                using (var stream = new StreamReader(filepath))
                {
                    FileText = stream.ReadToEnd();
                }
            }
            catch (Exception exc)
            {
                FileText = "Can't open file " + filepath;
            } 

            return View();
        }

        public ViewResult ThrowException()
        {
            Logger.Log.Debug("HomeController.ThrowException()");

            throw new HttpException(403, "Throw Exception");

            return View();
        }

        public ViewResult Flex()
        {
            Logger.Log.Debug("HomeController.Flex()");

            return View();
        }
    }

    public static class ListHelper
    {
        public static MvcHtmlString CreateList(this HtmlHelper html, string text)
        {
            string[] items = text.Split('\n');

            TagBuilder ul = new TagBuilder("ul");
            foreach (string item in items)
            {
                TagBuilder li = new TagBuilder("li");
                li.SetInnerText(item);
                ul.InnerHtml += li.ToString();
            }
            return new MvcHtmlString(ul.ToString());
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Banking
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //routes.MapRoute(
            //    null, // Route name
            //    "Home/User/Login", // URL with parameters
            //    new { controller = "User", action = "Login", id = UrlParameter.Optional });

            //routes.MapRoute(
            //    null, // Route name
            //    "Client/Home/Index", // URL with parameters
            //    new { controller = "Home", action = "Index" });

            //routes.MapRoute(
            //    "MyDefault", // Route name
            //    "User/Home/Index", // URL with parameters
            //    new { controller = "Home", action = "Index" });

            //routes.MapRoute(
            //    null, // Route name
            //    "Home/Client/List", // URL with parameters
            //    new { controller = "Client", action = "List" });

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                //defaults: new { controller = "Client", action = "List", id = UrlParameter.Optional }
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
           );

        }
    }
}
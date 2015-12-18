using Banking.Controllers;
using Banking.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace Banking
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AutofacConfig.ConfigureContainer();

            Logger.InitLogger();

            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        //protected void Application_Error(Object sender, EventArgs e)
        //{
        //   // Response.Redirect("~/Error/Index", true);

        //    try
        //    {
        //        // This is to stop a problem where we were seeing "gibberish" in the
        //        // chrome and firefox browsers
        //        HttpApplication app = sender as HttpApplication;
        //        app.Response.Filter = null;
        //    }
        //    catch
        //    {
        //    }
        //}
    }
}
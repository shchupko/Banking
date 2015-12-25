using Banking.Domain.Abstract;
using Banking.Mappers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace Banking.Controllers
{
    public abstract class DefaultController : Controller
    {
        public IMapper ModelMapper { get; set; }
        public static string HostName = string.Empty;

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            if (requestContext.HttpContext.Request.Url != null)
            {
                HostName = requestContext.HttpContext.Request.Url.Authority;
            }

            try
            {
                string lang = ConfigurationManager.AppSettings["Culture"] as string;
                var cultureInfo = new CultureInfo(lang);

                Thread.CurrentThread.CurrentCulture = cultureInfo;
                Thread.CurrentThread.CurrentUICulture = cultureInfo;
            }
            catch (Exception ex)
            {
                Logger.Log.ErrorFormat("Culture not found", ex);
            }

            base.Initialize(requestContext);
        }

        //protected override void OnException(ExceptionContext filterContext)
        //{
        //    if (!filterContext.ExceptionHandled)
        //    {
        //        if (filterContext == null)
        //        {
        //            throw new ArgumentNullException("filterContext");
        //        }

        //        // If custom errors are disabled, we need to let the normal ASP.NET exception handler
        //        // execute so that the user can see useful debugging information.
        //        if (filterContext.ExceptionHandled || !filterContext.HttpContext.IsCustomErrorEnabled)
        //        {
        //            return;
        //        }

        //        Exception exception = filterContext.Exception;

        //        //if (!ExceptionType.IsInstanceOfType(exception))
        //        //{
        //        //    return;
        //        //}

        //        string controllerName = (string)filterContext.RouteData.Values["controller"];
        //        string actionName = (string)filterContext.RouteData.Values["action"];
        //        HandleErrorInfo model = new HandleErrorInfo(filterContext.Exception, controllerName, actionName);
        //        filterContext.Result = new ViewResult
        //        {
        //            ViewName = "Error/Index",
        //           // MasterName = Master,
        //            ViewData = new ViewDataDictionary<HandleErrorInfo>(model),
        //            TempData = filterContext.Controller.TempData
        //        };
        //        filterContext.ExceptionHandled = true;
        //        filterContext.HttpContext.Response.Clear();
        //        filterContext.HttpContext.Response.StatusCode = new HttpException(null, exception).GetHttpCode();

        //        // Certain versions of IIS will sometimes use their own error page when
        //        // they detect a server error. Setting this property indicates that we
        //        // want it to try to render ASP.NET MVC's error page instead.
        //        filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;

        //        //RedirectToAction("Index", "Error", ex);
        //    }
        //}
    }

}

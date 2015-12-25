using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Banking.Domain.Abstract;
using System.Web.Security;
using System.Web;
using System.Web.Mvc;
using Banking.Domain.Models.ViewModels;

namespace Banking.Domain.Concrete
{
    public class FormAuthProvider : IAuthProvider
    {
        public IUserSqlRepository Repository { get; set; }
        public FormAuthProvider(IUserSqlRepository repo)
        {
            Repository = repo;
        }

        public void SignOut()
        {
            FormsAuthentication.SignOut();
            FormsAuthentication.RedirectToLoginPage();
        }


        public bool Authenticate(UserLoginView model, out string msg)
        {
            bool result = FormsAuthentication.Authenticate(model.Login, model.Password);
            if (result)
            {
                FormsAuthentication.SetAuthCookie(model.Login, model.RememberMe);
            }

            if (Repository.Login(model.Login, model.Password, out msg))
            {
                FormsAuthentication.SetAuthCookie(model.Login, model.RememberMe);
                //CreateCookie(model.Login, false);
                return true;
            }

            return false;
        }

        //public HttpContext HttpContext { get; set; }
        //public static void LogOffUser(HttpContextBase httpContext)
        //{
        //    if (httpContext.Request.Cookies["__AUTH"] != null)
        //    {
        //        var cookie = new HttpCookie("__AUTH") { Expires = DateTime.Now.AddDays(-1) };

        //        httpContext.Response.Cookies.Add(cookie);
        //    }
        //}
        //private void CreateCookie(string userName, bool isPersistent = false)
        //{
        //    var ticket = new FormsAuthenticationTicket(
        //          1,
        //          userName,
        //          DateTime.Now,
        //          DateTime.Now.Add(FormsAuthentication.Timeout),
        //          isPersistent,
        //          string.Empty,
        //          FormsAuthentication.FormsCookiePath);

        //    // Encrypt the ticket.
        //    var encTicket = FormsAuthentication.Encrypt(ticket);

        //    // Create the cookie.
        //    var AuthCookie = new HttpCookie(cookieName)
        //    {
        //        Value = encTicket,
        //        Expires = DateTime.Now.Add(FormsAuthentication.Timeout)
        //    };
        //    if (HttpContext != null)
        //        HttpContext.Response.Cookies.Set(AuthCookie);
        //}

        //private User _currentUser;
        //private const string cookieName = "__AUTH_COOKIE";

        //public User CurrentUser
        //{
        //    get
        //    {
        //        if (_currentUser == null)
        //        {
        //            try
        //            {
        //                HttpCookie authCookie = HttpContext.Request.Cookies.Get(cookieName);
        //                if (authCookie != null && !string.IsNullOrEmpty(authCookie.Value))
        //                {
        //                    var ticket = FormsAuthentication.Decrypt(authCookie.Value);
        //                    _currentUser = Repository.GetUser(ticket.Name);// new UserProvider(ticket.Name, Repository);
        //                }
        //                else
        //                {
        //                    //_currentUser = new UserProvider(null, null);
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                //Logger.Log.Error("Failed authentication: " + ex.Message);
        //                //_currentUser = new UserProvider(null, null);
        //            }
        //        }
        //        return _currentUser;
        //    }
        //}

    }
}

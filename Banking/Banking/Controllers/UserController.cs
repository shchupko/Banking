using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Banking.Domain;
using Banking.Domain.Abstract;
using System.Web.Security;
using Banking.Controllers;
using Banking.Tools;
using System.Drawing.Imaging;
using Banking.Mappers;
using Banking.Domain.Models.ViewModels;


namespace Banking.Controllers
{
    public class UserController : DefaultController
    {
        IAuthProvider authProvider;

        public UserController(IAuthProvider auth, IRepository repo, IMapper mapper)
        {
            authProvider = auth;
            Repository = repo;
            ModelMapper = mapper;
        }

        [HttpGet]
        public ViewResult qtip()
        {
            Logger.Log.Debug("User.qtip()");
            return View("qtip");
        }

        [HttpGet]
        public ActionResult Register()
        {
            Logger.Log.Debug("User.Register()");
            var newUser = new UserRegisterView();
            return View(newUser);
        }

        [HttpPost]
        public ActionResult Register(UserRegisterView userRegView)
        {
            Logger.Log.DebugFormat("User.Register() HttpPost {0}", userRegView.Login);

            if (userRegView.Captcha != (string)Session[CaptchaImage.CaptchaValueKey])
            {
                ModelState.AddModelError("Captcha", "The text from the image entered incorrectly");
                Logger.Log.ErrorFormat("Captcha was {0} but enterd {1}", (string)Session[CaptchaImage.CaptchaValueKey], userRegView.Captcha);
            }
            var anyUser = Repository.Users.Any(p => string.Compare(p.Login, userRegView.Login) == 0);
            if (anyUser)
            {
                ModelState.AddModelError("Login", "This Login is already registered");
            }

            if (ModelState.IsValid)
            {
                var user = (User)ModelMapper.Map(userRegView, typeof(UserRegisterView), typeof(User));

                Repository.CreateUser(user);
                return RedirectToAction("Login");
            }
            return View(userRegView);
        }

        public ActionResult Captcha()
        {
            Session[CaptchaImage.CaptchaValueKey] = new Random(DateTime.Now.Millisecond).Next(1111, 9999).ToString();
            var ci = new CaptchaImage(Session[CaptchaImage.CaptchaValueKey].ToString(), 211, 50, "Arial");

            // Change the response headers to output a JPEG image.
            this.Response.Clear();
            this.Response.ContentType = "image/jpeg";

            // Write the image to the response stream in JPEG format.
            ci.Image.Save(this.Response.OutputStream, ImageFormat.Jpeg);

            // Dispose of the CAPTCHA image object.
            ci.Dispose();
            return null;
        }


        [HttpGet]
        public ViewResult Login()
        {
            Logger.Log.Debug("User.Login() HttpGet");
            return View("Login");
        }

        [HttpPost]
        public ActionResult Login(UserLoginView model, string returnUrl)
        {
            Logger.Log.DebugFormat("User.Login() HttpPost. RememberMe {0}, returnUrl {1}", model.RememberMe.ToString(), returnUrl);

            //Page.Validate();
            if (ModelState.IsValid)
            {
                string msg = null;
                if (authProvider.Authenticate(model, out msg))
                {
                    Logger.Log.InfoFormat("User {0} Authenticate succesful", model.Login);
                    //return RedirectToAction("List", "Client", model.Login);
                    return Redirect(returnUrl ?? Url.Action("List", "Client"));
                }
                else
                {
                    ModelState.AddModelError("", msg); //"Login or Pasword incorrect"
                    Logger.Log.Error(msg);
                    return View();
                }
            }
            else
            {
                Logger.Log.Warn("Credantials don't valid");
                return View();
            }
        }

        public ActionResult SignOut()
        {
            Logger.Log.DebugFormat("SignOut {0}", User.Identity.Name);
            authProvider.SignOut();
            return Redirect(Url.Action("Index", "Home"));
        }


    }
}

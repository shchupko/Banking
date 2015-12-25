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
using System.IO;
using Banking.Mappers;
using Banking.Domain.Models.ViewModels;
using Banking.Tests.Tools.Mail;


namespace Banking.Controllers
{
    public class UserController : DefaultController
    {
        IAuthProvider authProvider;
        public IUserSqlRepository Repository { get; set; }

        public UserController(IAuthProvider auth, IUserSqlRepository repo, IMapper mapper)
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


        [HttpPost]
        public ActionResult ForgotPassword(ForgotPasswordView user)
        {
            Logger.Log.DebugFormat("User.ForgotPassword() [HttpPost] email {0}", user.Email);

            if (string.IsNullOrEmpty(user.Email))
            {
                return View(user);
            }

            var userDb = Repository.GetUserByEmail(user.Email);

            if (null == userDb)
            {
                string msg = "Email absent in DB: " + user.Email;
                ModelState.AddModelError("Login", msg);
            }
            else
            {
                //userDb.Email = "smart358@ukr.net";

                NotifyMail.SendNotify("ForgotPassword", userDb.Email,
                    subject => string.Format(subject, HostName),
                    body => string.Format(body, userDb.Login, userDb.Password, HostName));

                ViewBag.Msg = "Email was sent";
            }
            return View(user);
        }

        [HttpGet]
        public ActionResult ForgotPassword()
        {
            Logger.Log.DebugFormat("User.ForgotPassword() [HttpGet] ");

            var newUser = new ForgotPasswordView();
            return View(newUser);
        }


        [HttpGet]
        public ActionResult Register(string login)
        {
            Logger.Log.DebugFormat("User.Register() [HttpGet] login {0}", login);

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

                NotifyMail.SendNotify("Register", user.Email,
                    subject => string.Format(subject, HostName),
                    body => string.Format(body, "", HostName));

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
            Logger.Log.Debug("User.Login() [HttpGet]");
            return View("Login");
        }

        [HttpPost]
        public ActionResult Login(UserLoginView model)
        {
            Logger.Log.DebugFormat("User.Login() [HttpPost]. RememberMe {0}", model.RememberMe.ToString());

            //Page.Validate();
            if (ModelState.IsValid)
            {
                string msg = null;
                if (authProvider.Authenticate(model, out msg))
                {
                    Logger.Log.InfoFormat("User {0} Authenticate succesful", model.Login);
                    return RedirectToAction("List", "Client");
                    //return Redirect(Url.Action("List", "Client"));
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
                return View("Login");
            }
        }

        public ActionResult SignOut()
        {
            Logger.Log.DebugFormat("SignOut {0}", User.Identity.Name);
            authProvider.SignOut();
            return Redirect(Url.Action("Index", "Home"));
        }

        //[Authorize]
        //public ActionResult SubscriptionTest()
        //{
        //    var mailController = new MailController();

        //    var email = mailController.Subscription("Привет, мир!", CurrentUser.Email);
        //    email.Deliver();
        //    return Content("OK");
        //}

        //[Authorize]
        //public ActionResult SubscriptionShow()
        //{
        //    var mailController = new MailController();
        //    var email = mailController.Subscription("Привет, мир!", CurrentUser.Email);

        //    using (var reader = new StreamReader(email.Mail.AlternateViews[0].ContentStream))
        //    {
        //        var content = reader.ReadToEnd();
        //        return Content(content);
        //    }
        //    return null;
        //}
    }
}

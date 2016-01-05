using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Banking.Domain;
using Banking.Domain.Abstract;
using Banking.Tools;
using System.Drawing.Imaging;
using Banking.Domain.Mail;
using Banking.Mappers;
using Banking.Domain.Models.ViewModels;



namespace Banking.Controllers
{
    public class UserController : DefaultController
    {
        public IAuthProvider authProvider;
        public IUserSqlRepository Repository { get; set; }

        public UserController(IAuthProvider auth = null, IUserSqlRepository repo = null, IMapper mapper = null, INotifyMail mail = null)
        {
            authProvider = auth;
            Repository = repo;
            ModelMapper = mapper;
            mailProvider = mail;
        }

    [HttpGet]
    public ViewResult qtip()
    {
        Logger.Log.Debug("User.qtip()");
        return View("qtip");
    }

    [HttpPost]
    public RedirectToRouteResult qtip(string login)
    {
        Logger.Log.Debug("User.qtip()");
        return RedirectToAction("Index", "Home");
    }

        [HttpPost]
        public ActionResult ForgotPassword(UserLoginView user)
        {
            Logger.Log.DebugFormat("User.ForgotPassword() [HttpPost] Login {0}", user.Login);

            if (string.IsNullOrEmpty(user.Login))
            {
                return View(user);
            }

            var userDb = Repository.GetUserByLogin(user.Login);

            if (null == userDb)
            {
                string msg = "User " + user.Login + " absent in DB.";
                ModelState.AddModelError("Login", msg);
            }
            else
            {
                mailProvider.SendNotify("ForgotPassword", userDb.Email,
                    subject => string.Format(subject, HostName),
                    body => string.Format(body, userDb.Login, userDb.Password, HostName));

                ViewBag.Msg = "Credentials was sent to user email";
            }
            return View(user);
        }

        [HttpGet]
        public ActionResult ForgotPassword()
        {
            Logger.Log.DebugFormat("User.ForgotPassword() [HttpGet] ");

            var newUser = new UserLoginView();
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

                if (!userRegView.SkipEmailConfirmation)
                {
                    mailProvider.SendNotify("Register", user.Email,
                        subject => string.Format(subject, HostName),
                        body => string.Format(body,
                            Url.Action("ConfirmAndUnblock", "User", new { Token = user.Guid},
                                Request.Url.Scheme), HostName));

                    return RedirectToAction("ConfirmEmail", "User", new { msg = "Please confirm your email", login = user.Login });
                }
                else
                {
                    return RedirectToAction("Login");
                }
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

            var user = new UserLoginView();
            return View("Login", user);
        }

        [HttpPost]
        public ActionResult Login(UserLoginView model)
        {
            Logger.Log.DebugFormat("User.Login() [HttpPost]. RememberMe {0}", model.RememberMe.ToString());

            //Page.Validate();
            if (ModelState.IsValid)
            {
                string msg;
                int attemptCounter;
                if (authProvider.Authenticate(model, out msg, out attemptCounter))
                {
                    Logger.Log.InfoFormat("User {0} Authenticate succesful", model.Login);
                    return RedirectToAction("List", "Client");
                }
                else
                {
                    ModelState.AddModelError("", msg); //"Login or Pasword incorrect"
                    Logger.Log.Error(msg);

                    // Send mail notification when blocked
                    if (attemptCounter == model.BlockedAttemptNumber)
                    {
                        var user = Repository.GetUserByLogin(model.Login);
                        if (null != user)
                        {
                            mailProvider.SendNotify("UserBlocked", user.Email,
                                subject => string.Format(subject, HostName),
                                body => string.Format(body,
                                    Url.Action("Confirm", "User", new {Token = user.Guid}), HostName));

                            msg = "Email sent";
                            ModelState.AddModelError("", msg);
                            Logger.Log.Error(msg);
                        }
                    }
                    return View(model);
                }
            }
            else
            {
                Logger.Log.Warn("Credantials don't valid");
                return View(model);
            }
        }

        public ActionResult SignOut()
        {
            Logger.Log.DebugFormat("SignOut {0}", User.Identity.Name);
            authProvider.SignOut();
            return Redirect(Url.Action("Index", "Home"));
        }

       // [HttpPost]
       // [AllowAnonymous]
       //// [ValidateAntiForgeryToken]
       // public async Task<ActionResult> Register(UserRegisterView model)
       // {
       //     Logger.Log.DebugFormat("User.Register() [HttpPost]. Email {0}", model.Email.ToString());
       //     if (ModelState.IsValid)
       //     {
       //         var user = new UserRegisterView() { Login = model.Login };
       //         user.Email = model.Email;
       //         //user.ConfirmedEmail = false;
       //         //var result = await UserManager.CreateAsync(user, model.Password);
       //         //if (result.Succeeded)
       //         {
       //             // наш email с заголовком письма
       //             MailAddress from = new MailAddress("somemail@gmail.com", "Web Registration");
       //             // кому отправляем
       //             MailAddress to = new MailAddress(user.Email);
       //             // создаем объект сообщения
       //             MailMessage m = new MailMessage(from, to);
       //             // тема письма
       //             m.Subject = "Email confirmation";
       //             // текст письма - включаем в него ссылку
       //             m.Body = string.Format("Для завершения регистрации перейдите по ссылке:" +
       //                             "<a href=\"{0}\" title=\"Подтвердить регистрацию\">{0}</a>",
       //                 Url.Action("ConfirmEmail", "Account", new { Token = user.Login, Email = user.Email }, Request.Url.Scheme));
       //             m.IsBodyHtml = true;
       //             // адрес smtp-сервера, с которого мы и будем отправлять письмо
       //             SmtpClient smtp = new System.Net.Mail.SmtpClient("smtp.gmail.com", 25);
       //             // логин и пароль
        //             smtp.Credentials = new System.Net.NetworkCredential("somemail@gmail.com", "password");
       //             smtp.Send(m);
                    
       //         }

       //     }return RedirectToAction("List", "Client", new { Email = model.Email });
       //     //return System.Web.UI.WebControls.View(model);
       // }

        [AllowAnonymous]
        [HttpGet]
        public ViewResult ConfirmEmail(string msg, string login)
        {
            Logger.Log.DebugFormat("ConfirmEmail({0}) [HttpGet]", login);
            ViewBag.Msg = msg;
            ViewBag.UserLogin = login;
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ViewResult ConfirmEmail(string login)
        {
            Logger.Log.DebugFormat("ConfirmEmail({0}) [HttpPost]", login);

            var nameValueCollection = HttpUtility.ParseQueryString(Request.UrlReferrer.Query);
            login = nameValueCollection["login"];
            if (string.IsNullOrEmpty(login))
            {
                ViewBag.Msg = "Have not Login.";
            }
            else
            {
                var user = Repository.GetUserByLogin(login);
                if (null != user)
                {
                    mailProvider.SendNotify("Register", user.Email,
                        subject => string.Format(subject, HostName),
                        body => string.Format(body,
                            Url.Action("ConfirmAndUnblock", "User", new {Token = user.Guid.Trim()},
                                Request.Url.Scheme), HostName));

                    ViewBag.Msg = "Email was sent";
                    ViewBag.UserLogin = user.Login;
                }
            }
            var userView = new UserLoginView();
            userView.Login = login;
            return View(userView);
        }

    
        // Called from email reference
        [AllowAnonymous]
        public RedirectToRouteResult ConfirmAndUnblock(string token)
        {
            Logger.Log.DebugFormat("User.ConfirmAndUnblock(). Token {0}", token);
            string str, strLogin = null;
            
            var user = Repository.GetUserByGuid(token);

            if (user != null)
            {
                user.isConfirmedEmail = true;
                user.IsBlock = false;
                Repository.UpdateUser(user);

                strLogin = user.Login;
                str = "Success. You can login now.";
            }
            else
            {
                str = "Wrong token. Where did you got it? Please register again.";
            }
           return RedirectToAction("ConfirmEmail", "User", new { msg = str, login =  strLogin });        
        }
    }
}

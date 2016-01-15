using System;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac.Integration.Mvc;
using Banking.Controllers;
using Banking.Domain;
using Banking.Domain.Abstract;
using Banking.Domain.Concrete;
using Banking.Domain.Mail;
using Banking.Domain.Models.ViewModels;
using Banking.IntegrationTest.Setup;
using Banking.Mappers;
using Banking.CommonTests;
using Banking.Tools;
using NUnit.Framework;


namespace Banking.IntegrationTest
{
    [TestFixture]
    public class UserControllerTest
    {
        public IntegrationTestSetupFixture Init;
        public IUserSqlRepository UserRepository;
        public IAuthProvider AuthProvider;
        public IMapper Mapper;
        public INotifyMail NotifyMail;

        public UserControllerTest()
        {
            Init = new IntegrationTestSetupFixture();
            Init.Setup();

            var db = new BankingDbDataContext();
            UserRepository = new UserSqlRepository(db);
            Mapper = new CommonMapper();
            AuthProvider = new FormAuthProvider(UserRepository);
            NotifyMail = new NotifyMail();
        }

        private string RandomString(int size)
        {
            size /= 2;
            StringBuilder builder = new StringBuilder();
            Random random = new Random((int)DateTime.Now.Ticks);
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            var s2 = new string(Enumerable.Range(0, size).Select(_ => (char)random.Next('a', 'z')).ToArray());
            builder.Append(s2);
            return builder.ToString();
        }

        [Test]
        public void CreateNormalUserCountPlusOne()
        {
            var controller = new UserController(AuthProvider, UserRepository, Mapper, NotifyMail);
            var countBefore = UserRepository.Users.Count();
            var httpContext = new MockHttpContext().Object;

            var route = new RouteData();
            route.Values.Add("controller", "User");
            route.Values.Add("action", "Register");
            route.Values.Add("area", "Default");

            ControllerContext context = new ControllerContext(new RequestContext(httpContext, route), controller);
            controller.ControllerContext = context;

            controller.Session.Add(CaptchaImage.CaptchaValueKey, "1111");

            var registerUserView = new UserRegisterView()
            {
                Email = "vs@googlemail.com",
                Password = "123456",
                ConfirmPassword = "123456",
                Login = RandomString(6),
                Address = "addr" + RandomString(6),
                Captcha = "1111",
                SkipEmailConfirmation = true
            };

            ValidatorTool.ValidateObject<UserRegisterView>(registerUserView);
            controller.Register(registerUserView);

            var countAfter = UserRepository.Users.Count();
            Assert.That(countBefore + 1, Is.EqualTo(countAfter));
        }

        [Test]
        public void Create100UsersNoAssert()
        {
            var controller = new UserController(AuthProvider, UserRepository, Mapper, NotifyMail);
            var httpContext = new MockHttpContext().Object;
            
            var route = new RouteData();
            route.Values.Add("controller", "User");
            route.Values.Add("action", "Register");
            route.Values.Add("area", "Default");

            ControllerContext context = new ControllerContext(new RequestContext(httpContext, route), controller);
            controller.ControllerContext = context;

            controller.Session.Add(CaptchaImage.CaptchaValueKey, "1111");

            for (int i = 0; i < 100; i++)
            {
                var registerUserView = new UserRegisterView()
                {
                    Login = RandomString(6),
                    Password = "123456",
                    ConfirmPassword = "123456",
                    Email = "vv@d.d", // Email.GetRandom(Name.GetRandom(), Surname.GetRandom()),
                    Address = "",
                    Captcha = "1111",
                    SkipEmailConfirmation = true,
                    //BirthdateDay = rand.Next(28) + 1,
                    //BirthdateMonth = rand.Next(12) + 1,
                    //BirthdateYear = 1970 + rand.Next(20)
                };
                controller.Register(registerUserView);
            }
        }
    }
}

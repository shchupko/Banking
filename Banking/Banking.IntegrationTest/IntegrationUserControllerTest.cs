using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Banking.Controllers;
using Banking.Domain.Concrete;
using Banking.Domain.Models.ViewModels;
using Banking.IntegrationTest.Setup;
using Banking.Tests.Mock;
using Banking.Tests.Mock.Http;
using Banking.Tests.Tools;
using Banking.Tools;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace Banking.IntegrationTest
{
    [TestFixture]
    public class UserControllerTest
    {
        public IntegrationTestSetupFixture Init;

        public UserControllerTest()
        {
            Init = new IntegrationTestSetupFixture();
            Init.Setup();
        }

        [Test]
        public void CreateUserCreateNormalUserCountPlusOne()
        {
            var db = new MockBankingDbDataContext();   
            var mockRepository = new UserSqlRepository(db.Object);
            var controller = new UserController(null, mockRepository, null);

            var countBefore = mockRepository.Users.Count();
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
                Login = "Name",
                Address = "addr",
                Captcha = "1111"
            };

            ValidatorTool.ValidateObject<UserRegisterView>(registerUserView);
            controller.Register(registerUserView);

            var countAfter = mockRepository.Users.Count();
            Assert.AreEqual(countBefore + 1, countAfter);
        }

        [Test]
        public void CreateUserCreate100UsersNoAssert()
        {
        //    var repository = DependencyResolver.Current.GetService<IRepository>();
        //    var controller =
        //        DependencyResolver.Current.GetService<LessonProject.Areas.Default.Controllers.UserController>();

        //    var httpContext = new MockHttpContext().Object;

        //    var route = new RouteData();

        //    route.Values.Add("controller", "User");
        //    route.Values.Add("action", "Register");
        //    route.Values.Add("area", "Default");

        //    ControllerContext context = new ControllerContext(new RequestContext(httpContext, route), controller);
        //    controller.ControllerContext = context;

        //    controller.Session.Add(CaptchaImage.CaptchaValueKey, "1111");

        //    var rand = new Random((int) DateTime.Now.Ticks);
        //    for (int i = 0; i < 100; i++)
        //    {
        //        var registerUserView = new UserView()
        //        {
        //            ID = 0,
        //            Email = Email.GetRandom(Name.GetRandom(), Surname.GetRandom()),
        //            Password = "123456",
        //            ConfirmPassword = "123456",
        //            Captcha = "1111",
        //            BirthdateDay = rand.Next(28) + 1,
        //            BirthdateMonth = rand.Next(12) + 1,
        //            BirthdateYear = 1970 + rand.Next(20)
        //        };
        //        controller.Register(registerUserView);
        //    }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Banking.Controllers;
using Banking.Domain.Abstract;
using Banking.Domain.Models.ViewModels;
using Banking.Tools;
using Moq;
using NUnit.Framework;
using System.ComponentModel.DataAnnotations;
using Banking.Domain;
using Banking.Domain.Concrete;
using Banking.Tests.Mock;
using Banking.Tests.Mock.Http;
using Banking.Tests.Setup;
using Banking.Tests.Tools;
using Assert = NUnit.Framework.Assert;

namespace Banking.Tests
{
    [TestFixture]
    public class UserControllerRegisterTest
    {

        public UserControllerRegisterTest()
        {
            //UnitTestSetupFixture.ConfigureContainer();            

            try
            {
                var repo = DependencyResolver.Current.GetService<IAuthProvider>();
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
        }

        [Test]
        public void RegisterValidationAttribute()
        {
            var model = new UserRegisterView
            {
                Login = "Name",
                Password = "12345",
                ConfirmPassword = "123456",
                Email = "q@m.com"
            };

            model.Login = "";
            var errors = ValidatorTool.GetValidationErrors(model);
            Assert.AreEqual(errors[0].ErrorMessage, "Enter Login");

            model.Login = "name";
            model.Password = "";
            errors = ValidatorTool.GetValidationErrors(model);
            Assert.AreEqual(errors[0].ErrorMessage, "Enter Password");

            model.ConfirmPassword = "";
            model.Password = "12345";
            errors = ValidatorTool.GetValidationErrors(model);
            Assert.AreEqual(errors[0].ErrorMessage, "Confirm Password");

            model.ConfirmPassword = "555";
            model.Email = "";
            errors = ValidatorTool.GetValidationErrors(model);
            Assert.AreEqual(errors[0].ErrorMessage, "Enter email");
        }

        [Test]
        public void RegisterValidationAttributeDifferentPassword_ExceptionCompare()
        {
            //act
            var model = new UserRegisterView
            {
                Login = "user",
                Password = "12345",
                ConfirmPassword = "123456",
                Email = "q@m.com"
            };
            try
            {
                ValidatorTool.ValidateObject<UserRegisterView>(model);
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOf<ValidatorException>(ex);
                Assert.IsInstanceOf<System.ComponentModel.DataAnnotations.CompareAttribute>(((ValidatorException)ex).Attribute);
            }
        }

        [Test]
        public void RegisterUserWithWrongCaptcha_ModelStateWithError()
        {
            var db = new MockBankingDbDataContext();            
            var mockRepository = new UserSqlRepository(db.Object);

            var controller = new UserController(null, mockRepository, null);

            var httpContext = new MockHttpContext().Object;
            ControllerContext context = new ControllerContext(new RequestContext(httpContext, new RouteData()), controller);
            controller.ControllerContext = context;
            controller.Session.Add(CaptchaImage.CaptchaValueKey, "2222");

            //act
            var model = new UserRegisterView
            {
                Login = "user",
                Password = "12345",
                ConfirmPassword = "12345",
                Email = "q@m.com",
                Captcha = "1111"
            };

            var result = controller.Register(model);
            Assert.AreEqual("The text from the image entered incorrectly", controller.ModelState["Captcha"].Errors[0].ErrorMessage);
        }

    }
}

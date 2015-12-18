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
using Banking.Tests.Mock.Http;
using Banking.Tests.Tools;
using Assert = NUnit.Framework.Assert;

namespace Banking.Tests
{
    [TestFixture]
    public class UserControllerTest
    {
        //IRepository repo;
        //public UserControllerTest()
        //{
        //    repo = DependencyResolver.Current.GetService<IRepository>();
        //}

        
        [Test]
        public void TestMethod1()
        {
            var controller = new UserController(null, null, null);

            ViewResult result = controller.Login();

            Assert.IsInstanceOf<ViewResult>(result);
            Assert.AreEqual("Login", result.ViewName);
        }

        [Test]
        public void Can_Login_With_Valid_Credentials()
        {
            var model = new UserLoginView
            {
                Login = "admin",
                Password = "12345"
            };

            // Организация - создание имитации поставщика аутентификации
            Mock<IAuthProvider> mock = new Mock<IAuthProvider>();
            string msg = null;
            mock.Setup(m => m.Authenticate(model, out msg)).Returns(true);

            UserController target = new UserController(mock.Object, null, null);

            // Действие - аутентификация
            ActionResult result = target.Login(model, "/MyURL");

            // Утверждение
            Assert.IsInstanceOf<RedirectResult>(result);
            Assert.AreEqual("/MyURL", ((RedirectResult) result).Url);
        }

        [Test]
        public void LoginValidationAttribute()
        {
            var model = new UserLoginView();

            var errors = GetValidationErrors(model);
            Assert.AreEqual(errors[0].ErrorMessage, "Enter Login");

            model.Login = "12345";  
            errors = GetValidationErrors(model);
            Assert.AreEqual(errors[0].ErrorMessage, "Enter Password");
        }

        [Test]
        public void Index_RegisterUserWithDifferentPassword_ExceptionCompare()
        {
            ////init
            //var controller = DependencyResolver.Current.GetService<Areas.Default.Controllers.UserController>();
            //var httpContext = new MockHttpContext().Object;
            //ControllerContext context = new ControllerContext(new RequestContext(httpContext, new RouteData()), controller);
            //controller.ControllerContext = context;

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
                Banking.Tests.Tools.Validator.ValidateObject<UserRegisterView>(model);
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOf<ValidatorException>(ex);
                Assert.IsInstanceOf<System.ComponentModel.DataAnnotations.CompareAttribute>(((ValidatorException)ex).Attribute);
            }
        }

        [Test]
        public void Index_RegisterUserWithWrongCaptcha_ModelStateWithError()
        {
            //init
            IRepository repo = DependencyResolver.Current.GetService<IRepository>();
            var controller = new UserController(null, repo, null);

            var httpContext = new MockHttpContext().Object;
            ControllerContext context = new ControllerContext(new RequestContext(httpContext, new RouteData()), controller);
            controller.ControllerContext = context;
            controller.Session.Add(CaptchaImage.CaptchaValueKey, "2222");

            //act
            var model = new UserRegisterView
            {
                Login = "user",
                Password = "12345",
                ConfirmPassword = "123456",
                Email = "q@m.com",
                Captcha = "1111"
            };

            var result = controller.Register(model);
            Assert.AreEqual("The text from the image entered incorrectly", controller.ModelState["Captcha"].Errors[0].ErrorMessage);
        }

        [Test]
        public void RegisterValidationAttribute()
        {
            var model = new UserRegisterView
            {
                Login = "12345",
                Password = "12345",
                ConfirmPassword = "12345",
                Email = "q@m.com"
            };

            model.Login = "";
            var errors = GetValidationErrors(model);
            Assert.AreEqual(errors[0].ErrorMessage, "Enter Login");

            model.Login = "12345";
            model.Password = "";
            errors = GetValidationErrors(model);
            Assert.AreEqual(errors[0].ErrorMessage, "Enter Password");

            model.Password = "12345";
            errors = GetValidationErrors(model);
            //Assert.AreEqual(errors[0].ErrorMessage, "Passwords don't same");
            //Html.ValidationMessage("Password")

            model.ConfirmPassword = "";
            errors = GetValidationErrors(model);
            //Assert.AreEqual(errors[0].ErrorMessage, "Passwords don't same");

            model.ConfirmPassword = "555";
            model.Email = "";
            errors = GetValidationErrors(model);
            Assert.AreEqual(errors[0].ErrorMessage, "Enter email");

            //model.Login = "12345";
            model.Password = "12345";
            model.ConfirmPassword = "12345";
            model.Email = "q@m.com";
            try
            {
                Banking.Tests.Tools.Validator.ValidateObject<UserRegisterView>(model);
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOf<ValidatorException>(ex);
                Assert.IsInstanceOf<System.ComponentModel.DataAnnotations.CompareAttribute>(((ValidatorException)ex).Attribute);
            }
        }


        private IList<ValidationResult> GetValidationErrors(object model)
        {
            var validationContext = new ValidationContext(model, null, null);
            var validationResults = new List<ValidationResult>();
            System.ComponentModel.DataAnnotations.Validator.TryValidateObject(model, validationContext, validationResults);
            return validationResults;
        }

        //[Test]
        //public void Register_UserWithDifferentPassword_ExceptionCompare()
        //{
        //    //init
        //    var controller = DependencyResolver.Current.GetService<Areas.Default.Controllers.UserController>();
        //    var httpContext = new MockHttpContext().Object;
        //    ControllerContext context = new ControllerContext(new RequestContext(httpContext, new RouteData()), controller);
        //    controller.ControllerContext = context;

        //    //act
        //    var registerUserView = new UserRegisterView()
        //    {
        //        Login = "User1",
        //        Password = "123456",
        //        ConfirmPassword = "1234567",
        //        Address = "address",
        //        Email = "user@sample.com",
        //        Captcha = "1111"
        //    };
        //    try
        //    {
        //        Validator.ValidateObject<UserRegisterView>(registerUserView);
        //    }
        //    catch (Exception ex)
        //    {
        //        Assert.IsInstanceOf<ValidatorException>(ex);
        //        Assert.IsInstanceOf<System.ComponentModel.DataAnnotations.CompareAttribute>(((ValidatorException)ex).Attribute);
        //    }
        //}

        //[Test]
        //public void Register_UserWithWrongCaptcha_ModelStateWithError()
        //{
        //    //init
        //    //var controller = DependencyResolver.Current.GetService<Areas.Default.Controllers.UserController>();
        //    var controller = new UserController(null, null, null);
        //    var httpContext = new MockHttpContext().Object;
        //    ControllerContext context = new ControllerContext(new RequestContext(httpContext, new RouteData()), controller);
        //    controller.ControllerContext = context;
        //    controller.Session.Add(CaptchaImage.CaptchaValueKey, "2222");

        //    //act
        //    var registerUserView = new UserRegisterView()
        //    {
        //        Login = "User1",
        //        Password = "123456",
        //        ConfirmPassword = "1234567",
        //        Address = "address",
        //        Email = "user@sample.com",
        //        Captcha = "1111"
        //    };

        //    var result = controller.Register(registerUserView);
        //    Assert.AreEqual("Текст с картинки введен неверно", controller.ModelState["Captcha"].Errors[0].ErrorMessage);
        //}
    }
}

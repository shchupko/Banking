
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Moq;
using Banking.Controllers;
using System.Web.Mvc;
using Banking.Domain.Abstract;
using Banking.Domain.Models.ViewModels;
using Banking.Tests.Tools;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;

namespace Banking.Tests
{
    [TestFixture]
    public class UserControllerLoginTest
    {
        [Test]
        public void LoginView()
        {
            var controller = new UserController(null, null, null);

            ViewResult result = controller.Login();

            Assert.IsInstanceOf<ViewResult>(result);
            Assert.AreEqual("Login", result.ViewName);
        }

        [Test]
        public void CanLoginWithValidCredentials()
        {
            var model = new UserLoginView
            {
                Login = "admin",
                Password = "12345"
            };

            // Организация - создание имитации поставщика аутентификации
            Mock<IAuthProvider> mock = new Mock<IAuthProvider>();
            string msg;
            int attemptCounter;
            mock.Setup(m => m.Authenticate(model, out msg, out attemptCounter)).Returns(true);

            var controller = new UserController(mock.Object, null, null);

            // Действие - аутентификация
            ActionResult result = controller.Login(model);

            // Утверждение
            Assert.IsInstanceOf<RedirectToRouteResult>(result);
            object resStr = "";
            ((RedirectToRouteResult)result).RouteValues.TryGetValue("action", out resStr);
            Assert.AreEqual("List", resStr);
            //Assert.AreEqual("List", ((RedirectResult)result).Url);
        }

        [Test]
        public void CanotLoginWithNotValidCredentials()
        {
            var model = new UserLoginView();
            //Mock<UserController> mock = new Mock<UserController>();
            //mock.Setup(m => m.ViewData.ModelState.IsValid).Returns(true);

            var controller = new UserController(null, null, null);
            controller.ViewData.ModelState.AddModelError("Login", "ErrorMessage");

            ActionResult result = controller.Login(model);

            Assert.IsInstanceOf<ViewResult>(result);
            Assert.AreEqual("Login", ((ViewResult)result).ViewName);
            Assert.IsFalse(((ViewResult)result).ViewData.ModelState.IsValid); 
        }

        [Test]
        public void LoginValidationAttribute()
        {
            var model = new UserLoginView();

            var errors = ValidatorTool.GetValidationErrors(model);
            Assert.AreEqual(errors[0].ErrorMessage, "Enter Login");

            model.Login = "12345";
            errors = ValidatorTool.GetValidationErrors(model);
            Assert.AreEqual(errors[0].ErrorMessage, "Enter Password");
        }

        [Test]
        public void CanLoginWithCorrectCredentials()
        {
            // Организация - создание модели представления
            // с правильными учетными данными
            var model = new UserLoginView
            {
                Login = "admin",
                Password = "12345"
            };            
            
            // Организация - создание имитации поставщика аутентификации
            Mock<IAuthProvider> mock = new Mock<IAuthProvider>();
            string msg;
            int attemptCounter;
            mock.Setup(m => m.Authenticate(model, out msg, out attemptCounter)).Returns(true);

            var controller = new UserController(mock.Object, null, null);

            // Действие - аутентификация
            ActionResult result = controller.Login(model);

            // Утверждение
            Assert.IsInstanceOf<RedirectToRouteResult>(result);
            object resStr = "";
            ((RedirectToRouteResult) result).RouteValues.TryGetValue("action", out resStr);
            Assert.AreEqual("List", resStr);
        }

        [Test]
        public void CannotLoginWithUncorrectCredentials()
        {
            // Организация - создание модели представления
            // с некорректными учетными данными
            var model = new UserLoginView
            {
                Login = "badUser",
                Password = "badPass"
            };            
            
            // Организация - создание имитации поставщика аутентификации
            Mock<IAuthProvider> mock = new Mock<IAuthProvider>();
            string msg;
            int attemptCounter;
            mock.Setup(m => m.Authenticate(model, out msg, out attemptCounter)).Returns(false);

            // Организация - создание контроллера
            var controller = new UserController(mock.Object, null, null);

            // Действие - аутентификация
            ActionResult result = controller.Login(model);

            // Утверждение
            Assert.IsInstanceOf<ViewResult>(result);
            Assert.IsFalse(((ViewResult)result).ViewData.ModelState.IsValid); //???
        }
    }
}

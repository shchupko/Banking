using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Banking.Domain;
using Moq;
using Banking.Controllers;
using System.Web.Mvc;
using Banking.Domain.Abstract;
using Banking.Domain.Models.ViewModels;
using NUnit.Framework;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Banking.Tests
{
    [TestFixture]
    public class AuthenticateTest
    {
        [Test]
        public void Can_Login_With_Valid_Credentials()
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
            string msg = null;
            mock.Setup(m => m.Authenticate(model, out msg)).Returns(true);

            UserController target = new UserController(mock.Object, null, null);

            // Действие - аутентификация
            ActionResult result = target.Login(model, "/MyURL");

            // Утверждение
            Assert.IsInstanceOfType(result, typeof(RedirectResult));
            Assert.AreEqual("/MyURL", ((RedirectResult)result).Url);
        }

        [Test]
        public void Cannot_Login_With_Invalid_Credentials()
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
            string msg = null;
            mock.Setup(m => m.Authenticate(model, out msg)).Returns(false);

            // Организация - создание контроллера
            UserController target = new UserController(mock.Object, null, null);

            // Действие - аутентификация
            ActionResult result = target.Login(model, "/MyURL");

            // Утверждение
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.IsFalse(((ViewResult)result).ViewData.ModelState.IsValid);
        }
    }
}

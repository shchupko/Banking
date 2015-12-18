using System;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using Banking.Domain.Abstract;
using Banking.Mappers;
using Banking.Tests.Mock;
using NUnit.Framework;


namespace Banking.Tests.Setup
{
    [TestFixture]
    public class UnitTestSetupFixture
    {
        protected static string Sandbox = "../../Sandbox";

        [OneTimeSetUp]
        public void Setup()
        {
            Console.WriteLine("=====START=====");
            
            //InitRepository();

            ConfigureContainer();
        }

        [TearDown]
        public void TearDown()
        {
            Console.WriteLine("=====FINISH======");

        }

        public static void ConfigureContainer()
        {
            // получаем экземпляр контейнера
            var builder = new ContainerBuilder();

            // регистрируем контроллер в текущей сборке
            builder.RegisterControllers(typeof(MvcApplication).Assembly);

            //builder.Register<BankingDbDataContext>(c => new BankingDbDataContext(ConfigurationManager.ConnectionStrings["BankingDb"].ConnectionString));

            builder.RegisterType<CommonMapper>().As<IMapper>().SingleInstance();
            //builder.RegisterType<FormAuthProvider>().As<IAuthProvider>();
            builder.RegisterType<MockRepository>().As<IRepository>().InstancePerRequest();

            // создаем новый контейнер с теми зависимостями, которые определены выше
            var container = builder.Build();

            // установка сопоставителя зависимостей
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}

//[Test]
//public void TestAanhefSelectListItems()
//{
//    var repository = container.Resolve<IRepository>();
//    Assert.IsTrue(repository.AanhefSelectListItems(0).Count > 0);
//}
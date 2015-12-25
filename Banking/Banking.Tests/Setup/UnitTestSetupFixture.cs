using System;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using Banking.Domain;
using Banking.Domain.Abstract;
using Banking.Domain.Concrete;
using Banking.Mappers;
using Banking.Tests.Mock;
using Moq;
using NUnit.Framework;


namespace Banking.Tests.Setup
{
    [TestFixture] //???
    public class UnitTestSetupFixture
    {
        [OneTimeSetUp]
        public virtual void Setup()
        {
            Console.WriteLine("=====START=====");
            
            ConfigureContainer();
        }

        [OneTimeTearDown]
        public virtual void TearDown()
        {
            Console.WriteLine("=====FINISH======");

        }

        protected virtual void ConfigureContainer()
        {
            var builder = new ContainerBuilder();

            builder.RegisterControllers(typeof(MvcApplication).Assembly);

            InitRepository(builder);

            builder.RegisterType<CommonMapper>().As<IMapper>().SingleInstance();
            builder.RegisterType<FormAuthProvider>().As<IAuthProvider>();
            //builder.RegisterType<MockRepositoryUser>().As<IUserSqlRepository>().InstancePerRequest();
            //builder.RegisterType<ClientSqlRepository>().As<IClientSqlRepository>().InstancePerRequest();
           
           // builder.RegisterInstance(HttpRequestScopedFactoryFor<IUserSqlRepository>()); 
            builder.RegisterModule(new AutofacWebTypesModule());

            // создаем новый контейнер с теми зависимостями, которые определены выше
            var container = builder.Build();

            // установка сопоставителя зависимостей
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }

        protected virtual void InitRepository(ContainerBuilder builder)
        {
            // builder.RegisterType<MockSqlRepository>().As<ISqlRepository>().InstancePerRequest();
            builder.Register<BankingDbDataContext>(c => new MockBankingDbDataContext().Object).SingleInstance();

        }
    }
}

//[Test]
//public void TestAanhefSelectListItems()
//{
//    var repository = container.Resolve<IRepository>();
//    Assert.IsTrue(repository.AanhefSelectListItems(0).Count > 0);
//}
using Autofac;
using Autofac.Core;
using Autofac.Integration.Mvc;
using Banking.Domain;
using Banking.Domain.Abstract;
using Banking.Domain.Concrete;
using Banking.Mappers;
using System.Configuration;
using System.Web.Mvc;

namespace Banking.Tools
{
    public class AutofacConfig
    {
        public static void ConfigureContainer()
        {
            // получаем экземпляр контейнера
            var builder = new ContainerBuilder();
 
            // регистрируем контроллер в текущей сборке
            builder.RegisterControllers(typeof(MvcApplication).Assembly);
 
            // регистрируем споставление типов 
            //kernel.Bind<BankingDbDataContext>().ToMethod(c => new BankingDbDataContext(ConfigurationManager.ConnectionStrings["EFDbContext"].ConnectionString));
            builder.Register<BankingDbDataContext>(c => new BankingDbDataContext(ConfigurationManager.ConnectionStrings["BankingDb"].ConnectionString));

            //kernel.Bind<IMapper>().To<CommonMapper>().InSingletonScope();
            //kernel.Bind<IAuthProvider>().To<FormAuthProvider>();
            //kernel.Bind<IUserRepository>().To<SqlRepository>().InRequestScope();
            builder.RegisterType<CommonMapper>().As<IMapper>().SingleInstance();
            builder.RegisterType<FormAuthProvider>().As<IAuthProvider>();
            builder.RegisterType<SqlRepository>().As<IRepository>().InstancePerRequest();


            // создаем новый контейнер с теми зависимостями, которые определены выше
            var container = builder.Build();
 
            // установка сопоставителя зависимостей
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }

    }


}
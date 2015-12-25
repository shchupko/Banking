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

            var connectionstring = ConfigurationManager.ConnectionStrings["BankingDb"].ConnectionString;
            builder.Register<BankingDbDataContext>(c => new BankingDbDataContext(connectionstring));


            builder.RegisterType<CommonMapper>().As<IMapper>().SingleInstance();
            builder.RegisterType<FormAuthProvider>().As<IAuthProvider>();
            builder.RegisterType<UserSqlRepository>().As<IUserSqlRepository>().InstancePerRequest();
            builder.RegisterType<ClientSqlRepository>().As<IClientSqlRepository>().InstancePerRequest();

            // создаем новый контейнер с теми зависимостями, которые определены выше
            var container = builder.Build();
 
            // установка сопоставителя зависимостей
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }

    }


}
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Autofac;
using NUnit.Framework;
using Banking.Tests.Mock;
using Banking.Tests.Setup;
using System.Configuration;
using Autofac.Integration.Mvc;
using Banking.Domain.Abstract;
using Banking.Domain.Concrete;
using Banking.Mappers;

namespace Banking.IntegrationTest.Setup
{
    [TestFixture]
    public class IntegrationTestSetupFixture/* : UnitTestSetupFixture*/
    {
        protected static string Sandbox = "../../Sandbox";

        public class FileListRestore
        {
            public string LogicalName { get; set; }
            public string Type { get; set; }
        }

        protected static bool removeDbAfter = false;

        protected static string NameDb = "BankingDb";

        protected static string TestDbName;


        [OneTimeSetUp]
        public virtual void Setup()
        {
            Console.WriteLine("=====START=====");

            ConfigureContainer();
        }


        protected void ConfigureContainer()
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


        protected void InitRepository(ContainerBuilder builder)
        {
            FileInfo sandboxFile;
            string connectionString;
            CopyDb(out sandboxFile, out connectionString);

            //builder.Register<BankingDbDataContext>(c => new MockBankingDbDataContext().Object).SingleInstance();

            sandboxFile.Delete();
        }

        private void CopyDb(out FileInfo sandboxFile, out string connectionString)
        {
            //var config = kernel.Get<IConfig>();
            var connectionstring = ConfigurationManager.ConnectionStrings["BankingDb"].ConnectionString;
            var db = new DataContext(connectionstring);

            TestDbName = string.Format("{0}_{1}", NameDb, DateTime.Now.ToString("yyyyMMdd_HHmmss"));

            Console.WriteLine("Create DB = " + TestDbName);
            sandboxFile = new FileInfo(string.Format("{0}\\{1}.bak", Sandbox, TestDbName));
            var sandboxDir = new DirectoryInfo(Sandbox);

            //backupFile
            var textBackUp = string.Format(@"-- Backup the database
            BACKUP DATABASE [{0}]
            TO DISK = '{1}'
            WITH COPY_ONLY",
            NameDb, sandboxFile.FullName);
            db.ExecuteCommand(textBackUp);

            var restoreFileList = string.Format("RESTORE FILELISTONLY FROM DISK = '{0}'", sandboxFile.FullName);
            var fileListRestores = db.ExecuteQuery<FileListRestore>(restoreFileList).ToList();
            var logicalDbName = fileListRestores.FirstOrDefault(p => p.Type == "D");
            var logicalLogDbName = fileListRestores.FirstOrDefault(p => p.Type == "L");

            var restoreDb = string.Format("RESTORE DATABASE [{0}] FROM DISK = '{1}' WITH FILE = 1, MOVE N'{2}' TO N'{4}\\{0}.mdf', MOVE N'{3}' TO N'{4}\\{0}.ldf', NOUNLOAD, STATS = 10", TestDbName, sandboxFile.FullName, logicalDbName.LogicalName, logicalLogDbName.LogicalName, sandboxDir.FullName);
            db.ExecuteCommand(restoreDb);

            connectionString = connectionstring.Replace(NameDb, TestDbName);
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            if (removeDbAfter)
            {
                RemoveDb();
            }
            Console.WriteLine("===============");
            Console.WriteLine("=====BYE!======");
            Console.WriteLine("===============");
        }


        private void RemoveDb()
        {
            var connectionstring = ConfigurationManager.ConnectionStrings["BankingDb"].ConnectionString;

            var db = new DataContext(connectionstring);

            var textCloseConnectionTestDb = string.Format(@"ALTER DATABASE [{0}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE", TestDbName);
            db.ExecuteCommand(textCloseConnectionTestDb);

            var textDropTestDb = string.Format(@"DROP DATABASE [{0}]", TestDbName);
            db.ExecuteCommand(textDropTestDb);
        }
    }
}

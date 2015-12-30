
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Banking.Domain;
using Banking.Domain.Abstract;
using Moq;

namespace Banking.Tests.Mock
{
    public class MockDb
    {
        public List<User> Users { get; set; }
        public List<Client> Clients { get; set; }
    }

    public class MockBankingDbDataContext : Mock<BankingDbDataContext>
    {
        public MockDb Db;

        public MockBankingDbDataContext(MockBehavior mockBehavior = MockBehavior.Strict)
            : base(mockBehavior)
        {
            Db = new MockDb();
             
            GenerateUsers();
            GenerateClients();
        }

        public void GenerateClients() { }
        public void GenerateUsers()
        {
            Db.Users = new List<User>();

            Db.Users.Add(new User()
            {
                Id = 1,
                Login = "user1",
                Password = "123456",
                Email = "volodymyr@gmail.com",
                Lastname = "Lastname1",
                //Cookies;
                AttemptCounter = 2,
                //RegDate;
                IsBlock = false
            });

            //this.Setup(p => p.Users.Context.SubmitChanges).Returns(Db.Users.AsQueryable());

            //this.Setup(p => p.Users).Returns(Db.Users.AsQueryable());

            //this.Setup(p => p.GetUserByLogin(It.IsAny<string>())).Returns((string name) =>
            //    Db.Users.FirstOrDefault(p => string.Compare(p.Email, email, 0) == 0));

            //string outMsg = "";
            //this.Setup(p => p.Login(It.IsAny<string>(), It.IsAny<string>(), out outMsg)).Returns(("outMsg") =>
            //    Users.FirstOrDefault(p => string.Compare(p.Login, login, 0) == 0));
        }
    }
}



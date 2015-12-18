using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using Banking.Domain;
using Moq;

namespace Banking.Tests.Mock
{
    public partial class MockRepository
    {
        public List<User> Users { get; set; }

        public void GenerateClients() { }
        public void GenerateUsers()
        {
            Users = new List<User>();

            Users.Add(new User()
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


            this.Setup(p => p.Users).Returns(Users.AsQueryable());

            this.Setup(p => p.GetUser(It.IsAny<string>())).Returns((string email) => 
                Users.FirstOrDefault(p => string.Compare(p.Email, email, 0) == 0));

            //string outMsg = "";
            //this.Setup(p => p.Login(It.IsAny<string>(), It.IsAny<string>(), out outMsg)).Returns(("outMsg") =>
            //    Users.FirstOrDefault(p => string.Compare(p.Login, login, 0) == 0));
        }
    }
}

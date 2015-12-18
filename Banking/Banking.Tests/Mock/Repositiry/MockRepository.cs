
using System.Collections;
using System.Collections.Generic;
using Banking.Domain;
using Banking.Domain.Abstract;
using Moq;

namespace Banking.Tests.Mock
{
    public partial class MockRepository : Mock<IRepository>
    {
        public MockRepository(MockBehavior mockBehavior = MockBehavior.Strict)
            : base(mockBehavior)
        {
            GenerateUsers();
            GenerateClients();
        }
    }
}

/*IRepository
bool CreateUser(User instance);
bool UpdateUser(User instance);
bool RemoveUser(int idUser);
User GetUser(string email);
bool Login(string login, string password, out string msg);
IEnumerable<Client> Clients { get; }
bool CreateClient(Client instance);
bool UpdateClient(Client instance);
bool RemoveClient(int idUser);*/
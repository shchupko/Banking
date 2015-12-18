using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Banking.Domain.Abstract
{
    public interface IRepository
    {
        IEnumerable<User> Users { get; }

        bool CreateUser(User instance);

        bool UpdateUser(User instance);

        bool RemoveUser(int idUser);

        User GetUser(string email);

        bool Login(string login, string password, out string msg);


        IEnumerable<Client> Clients { get; }

        bool CreateClient(Client instance);

        bool UpdateClient(Client instance);

        bool RemoveClient(int idUser);
    }


}
